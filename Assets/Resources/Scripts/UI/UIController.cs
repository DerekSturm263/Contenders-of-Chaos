using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class UIController : MonoBehaviour
{
    public TMPro.TMP_Text roomNumber;

    public GameObject codePrompt;

    public TMPro.TMP_InputField codeInput;
    public TMPro.TMP_InputField usernameInput;

    public TMPro.TMP_Text enterCodePrompt;

    public static UIController GetActiveController()
    {
        return GameObject.FindObjectOfType<UIController>();
    }

    public void UpdateRoomNumber(string roomNum)
    {
        roomNumber.text = "Room Code: " + roomNum;
    }

    public void HostGame()
    {
        CloudGameData.isHosting = true;
        SceneManager.LoadScene("Team Select");
    }

    public void JoinGame()
    {
        CloudGameData.isHosting = false;
        DisplayCodePrompt();
    }

    private void DisplayCodePrompt()
    {
        codePrompt.SetActive(true);
        enterCodePrompt.text = "Please enter the room code of the game you wish to enter.\n\nExample: 123456";
    }

    public void HideCodePrompt()
    {
        codePrompt.SetActive(false);
    }

    public void EnterCode()
    {
        string input = codeInput.text;

        if (input.Length == 6)
        {
            StartCoroutine(TryJoin(input));
        }
        else
        {
            enterCodePrompt.text = "Please enter the room code of the game you wish to enter.\n\nPlease type in a valid room code.";
        }
    }

    public void EnterUserName()
    {
        string input = usernameInput.text;

        GameController.userName = input;
    }

    public void GoToPlay()
    {
        SceneManager.LoadScene("Host or Join Game");
    }

    private IEnumerator TryJoin(string input)
    {
        for (int i = 10; i < 20; ++i)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(CloudDataController.PullURL + i))
            {
                yield return webRequest.SendWebRequest();

                string[] pages = i.ToString().Split('/');
                int page = pages.Length - 1;

                if (webRequest.isNetworkError)
                {
                    Debug.LogError("An error has occurred while pulling.\n" + webRequest.error);
                }
                else
                {
                    if (webRequest.downloadHandler.text.Contains(input))
                    {
                        CloudGameData.roomNum = input;
                        CloudGameData.gameNum = i - 10;

                        SceneManager.LoadScene("Team Select");
                        break;
                    }
                }
            }
        }

        enterCodePrompt.text = "Please enter the room code of the game you wish to enter.\n\nThere are no open rooms with that room code.";
    }

    public void QuitGame()
    {
        if (CloudGameData.isHosting)
        {
            CloseGame();
        }
        else
        {
            LeaveGame();
        }
    }

    private void CloseGame()
    {

    }

    public void LeaveToTitle()
    {
        if (codePrompt.activeSelf)
        {
            HideCodePrompt();
        }
        else
        {
            SceneManager.LoadScene("Title");
        }
    }

    private void LeaveGame()
    {
        TeamsUpdater thisTeam = TeamsUpdater.GetTeamsUpdater();

        StartCoroutine(thisTeam.LeaveTeam(Array.IndexOf(thisTeam.teams, thisTeam.thisPlayer)));
        SceneManager.LoadScene("Host or Join Game");
    }
}
