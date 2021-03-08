using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Linq;
using System;

public class TeamsUpdater : MonoBehaviour
{
    public GameObject[] teamObjs = new GameObject[4];
    public static Team[] teams = new Team[4];

    public bool isInTeam = false;
    public bool update = false;

    public GameObject hostCloseGame;

    private void Awake()
    {
        teamObjs.ToList().ForEach(x => teams[Array.IndexOf(teamObjs, x)] = x.GetComponent<Team>());

        StartCoroutine(UpdateTeams());
        StartCoroutine(CheckForStart());
        StartCoroutine(CheckForClose());
    }

    public static TeamsUpdater GetTeamsUpdater()
    {
        return FindObjectOfType<TeamsUpdater>();
    }

    public IEnumerator UpdateTeam(int playerNum)
    {
        bool removePlayer = false;
        Team thisTeam = null; // Should return error.
        PlayerData p = null; // Should return error.

        UIController.GetActiveController().UpdateRoomNumber(CloudGameData.roomNum);

        using (UnityWebRequest webRequest = UnityWebRequest.Get(CloudGameData.PullURL + (playerNum + (10 * (CloudGameData.gameNum + 1) + 10))))
        {
            yield return webRequest.SendWebRequest();

            string[] pages = (playerNum / 2).ToString().Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.LogError("An error has occurred while pulling.\n" + webRequest.error);
            }
            else
            {
                if (webRequest.downloadHandler.text.Contains("*PC*"))
                {
                    Debug.Log(webRequest.downloadHandler.text + " has succesfully joined Team " + playerNum / 2);
                    string playerName = webRequest.downloadHandler.text.Replace("*PC*", "");
                    playerName = playerName.Replace(GetIndexOfPlayer(playerNum / 2, 0, CloudGameData.gameNum) + ",", "");

                    PlayerData newPlayer = new PlayerData(playerName, PlayerData.Device_Type.PC);

                    teams[playerNum / 2].AddPlayer(newPlayer);
                }
                else if (webRequest.downloadHandler.text.Contains("*MB*"))
                {
                    Debug.Log(webRequest.downloadHandler.text + " has succesfully joined Team " + playerNum / 2);
                    string playerName = webRequest.downloadHandler.text.Replace("*MB*", "");
                    playerName = playerName.Replace(GetIndexOfPlayer(playerNum / 2, 1, CloudGameData.gameNum) + ",", "");

                    PlayerData newPlayer = new PlayerData(playerName, PlayerData.Device_Type.MB);

                    teams[playerNum / 2].AddPlayer(newPlayer);
                }
                else if (webRequest.downloadHandler.text.Contains("*L*"))
                {
                    Debug.Log(webRequest.downloadHandler.text + " has left the game.");

                    thisTeam = teams[playerNum / 2];

                    p = thisTeam.GetPlayers()[playerNum % 2 == 0 ? 0 : 1];

                    removePlayer = true;
                }
            }

            teams[playerNum / 2].UpdateInfo();

            int teamsFull = 0;

            for (int i = 0; i < 4; i++)
            {
                if (!teams[i].IsEmpty())
                {
                    teamsFull++;
                }
            }

            if (CloudGameData.isHosting && teamsFull > 1)
            {
                UIController.GetActiveController().startGameButton.SetActive(true);
            }
            else
            {
                UIController.GetActiveController().startGameButton.SetActive(false);
            }
        }

        if (removePlayer)
        {
            thisTeam.RemovePlayer(p);

            WWWForm form = new WWWForm();
            form.AddField("groupid", "pm36");
            form.AddField("grouppw", "N3Km3yJZpM");
            form.AddField("row", GetIndexOfPlayer(playerNum / 2, (int) p.deviceType, CloudGameData.gameNum));
            form.AddField("s4", "E");

            using (UnityWebRequest webRequest = UnityWebRequest.Post(CloudGameData.PushURL, form))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.isNetworkError)
                {
                    Debug.LogError("An error has occurred while pushing.\n" + webRequest.error);
                }
            }
        }

        if (update)
        {
            StartCoroutine(UpdateTeam(playerNum));
        }
    }

    public IEnumerator UpdateTeams()
    {
        for (int i = 0; i < 8; ++i)
        {
            StartCoroutine(UpdateTeam(i));
            yield return null;
        }
    }

    public IEnumerator CheckForStart()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(CloudGameData.PullURL + (220 + CloudGameData.gameNum)))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.downloadHandler.text.Contains("True"))
            {
                SceneManager.LoadScene("Main");
            }
        }

        if (update)
        {
            StartCoroutine(CheckForStart());
        }
    }

    public IEnumerator CheckForClose()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(CloudGameData.PullURL + (CloudGameData.gameNum)))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.downloadHandler.text.Contains("False"))
            {
                hostCloseGame.SetActive(true);
            }
        }

        if (update)
        {
            StartCoroutine(CheckForClose());
        }
    }

    public static int GetIndexOfPlayer(int teamNum, int deviceType, int gameNum)
    {
        return teamNum * 2 + deviceType + (10 * (gameNum + 1) + 10);
    }

    public static int GetIndexOfPlayerPosition(int teamNum, int deviceType, int gameNum)
    {
        return teamNum * 2 + deviceType + (10 * (gameNum + 1) + 10) + 100;
    }

    public static int GetIndexOfPlayerState(int teamNum, int deviceType, int gameNum)
    {
        return teamNum * 2 + deviceType + (10 * (gameNum + 1) + 10) + 230;
    }
}
