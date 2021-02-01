using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class UIController : MonoBehaviour
{
    public TMPro.TMP_Text roomNumber;

    public GameObject codePrompt;

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
    }

    public void HideCodePrompt()
    {
        codePrompt.SetActive(false);
    }

    public void EnterCode()
    {
        string input = EventSystem.current.currentSelectedGameObject.GetComponent<TMPro.TMP_InputField>().text;

        if (input.Length == 6)
        {
            StartCoroutine(TryJoin(input));
        }
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

                if (webRequest.isDone)
                {
                    if (webRequest.downloadHandler.text.Contains(input))
                    {
                        Debug.Log("Data: " + webRequest.downloadHandler.text + " has been succesfully pulled from database.");
                        CloudGameData.roomNum = input;

                        SceneManager.LoadScene("Team Select");
                        break;
                    }
                }
                else
                {
                    Debug.LogError("An error has occurred while pulling.\n" + webRequest.error);
                }
            }
        }
    }
}
