using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class TeamsUpdater : MonoBehaviour
{
    [HideInInspector] public GameObject[] teams = new GameObject[4];

    public bool isInTeam = false;

    private void Awake()
    {
        teams = GameObject.FindGameObjectsWithTag("Team");
        StartCoroutine(UpdateTeams());
    }

    public static TeamsUpdater GetTeamsUpdater()
    {
        return GameObject.FindObjectOfType<TeamsUpdater>();
    }

    public IEnumerator UpdateTeams()
    {
        UIController.GetActiveController().UpdateRoomNumber(CloudGameData.roomNum);

        for (int i = 0; i < 8; ++i)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(CloudGameData.PullURL + (i + (10 * (CloudGameData.gameNum + 1) + 10))))
            {
                yield return webRequest.SendWebRequest();

                string[] pages = i.ToString().Split('/');
                int page = pages.Length - 1;

                int index = GetIndexOfPlayer(i / 2, (int) GameController.playerInfo.deviceType, CloudGameData.gameNum);

                if (webRequest.isNetworkError)
                {
                    Debug.LogError("An error has occurred while pulling.\n" + webRequest.error);
                }
                else
                {
                    if (webRequest.downloadHandler.text.Contains("*PC*"))
                    {
                        Debug.Log(webRequest.downloadHandler.text + " has succesfully joined Team " + i / 2 + " at index: " + index);
                        string playerName = webRequest.downloadHandler.text.Replace("*PC*", "");

                        PlayerData newPlayer = new PlayerData(playerName, PlayerData.Device_Type.PC);

                        teams[i / 2].GetComponent<Team>().AddPlayer(newPlayer);
                    }
                    else if (webRequest.downloadHandler.text.Contains("*MB*"))
                    {
                        Debug.Log(webRequest.downloadHandler.text + " has succesfully joined Team " + i / 2 + " at index: " + index);
                        string playerName = webRequest.downloadHandler.text.Replace("*MB*", "");

                        PlayerData newPlayer = new PlayerData(playerName, PlayerData.Device_Type.MB);

                        teams[i / 2].GetComponent<Team>().AddPlayer(newPlayer);
                    }
                }

                teams[i / 2].GetComponent<Team>().UpdateInfo();
            }
        }
    }

    public static int GetIndexOfPlayer(int teamNum, int deviceType, int gameNum)
    {
        return teamNum * 2 + deviceType + (10 * (gameNum + 1) + 10);
    }
}
