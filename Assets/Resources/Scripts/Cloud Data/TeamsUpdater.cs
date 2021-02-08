using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class TeamsUpdater : MonoBehaviour
{
    public GameObject[] teams = new GameObject[4];

    public bool isInTeam = false;
    public bool update = false;

    private void Awake()
    {
        StartCoroutine(UpdateTeams());
    }

    public static TeamsUpdater GetTeamsUpdater()
    {
        return GameObject.FindObjectOfType<TeamsUpdater>();
    }

    public IEnumerator UpdateTeam(int playerNum)
    {
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

                        teams[playerNum / 2].GetComponent<Team>().AddPlayer(newPlayer);
                    }
                    else if (webRequest.downloadHandler.text.Contains("*MB*"))
                    {
                        Debug.Log(webRequest.downloadHandler.text + " has succesfully joined Team " + playerNum / 2);
                        string playerName = webRequest.downloadHandler.text.Replace("*MB*", "");
                        playerName = playerName.Replace(GetIndexOfPlayer(playerNum / 2, 1, CloudGameData.gameNum) + ",", "");

                        PlayerData newPlayer = new PlayerData(playerName, PlayerData.Device_Type.MB);

                        teams[playerNum / 2].GetComponent<Team>().AddPlayer(newPlayer);
                    }
                }

                teams[playerNum / 2].GetComponent<Team>().UpdateInfo();
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

    public static int GetIndexOfPlayer(int teamNum, int deviceType, int gameNum)
    {
        return teamNum * 2 + deviceType + (10 * (gameNum + 1) + 10);
    }
}
