using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class TeamsUpdater : MonoBehaviour
{
    [HideInInspector] public GameObject[] teams = new GameObject[4];
    [HideInInspector] public PlayerData thisPlayer;
    public bool isInTeam = false;

    private void Awake()
    {
        thisPlayer = new PlayerData(GameController.userName);
    }

    private void Start()
    {
        teams = GameObject.FindGameObjectsWithTag("Team");
    }

    public static TeamsUpdater GetTeamsUpdater()
    {
        return GameObject.FindObjectOfType<TeamsUpdater>();
    }

    public IEnumerator UpdateTeams()
    {
        for (int i = 0; i < 8; ++i)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(CloudDataController.PullURL + (i + (10 * (CloudGameData.gameNum + 1) + 10))))
            {
                yield return webRequest.SendWebRequest();

                string[] pages = i.ToString().Split('/');
                int page = pages.Length - 1;

                int index = GetIndexOfPlayer(i / 2, (int) thisPlayer.deviceType, CloudGameData.gameNum);

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

        StartCoroutine(JoinTeam(GetNextTeam()));
    }

    private IEnumerator JoinTeam(int teamNum)
    {
        isInTeam = true;
        teams[teamNum].GetComponent<Team>().AddPlayer(thisPlayer);
        int index = GetIndexOfPlayer(teamNum, (int) thisPlayer.deviceType, CloudGameData.gameNum);
        string addition = (thisPlayer.deviceType == PlayerData.Device_Type.PC) ? "*PC*" : "*MB*";

        WWWForm form = new WWWForm();
        form.AddField("groupid", "vgd21");
        form.AddField("grouppw", "foobar21");
        form.AddField("row", index);
        form.AddField("s4", addition + thisPlayer.name);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(CloudDataController.PushURL, form))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.LogError("An error has occurred while pushing.\n" + webRequest.error);
            }
            else
            {
                
;               Debug.Log("You (" + thisPlayer.name + ") have succesfully joined Team " + teamNum + " at index: " + index);
            }
        }

        teams[teamNum].GetComponent<Team>().UpdateInfo();
    }

    public IEnumerator LeaveTeam(int teamNum)
    {
        teams[teamNum].GetComponent<Team>().RemovePlayer(thisPlayer);

        WWWForm form = new WWWForm();
        form.AddField("groupid", "vgd21");
        form.AddField("grouppw", "foobar21");
        form.AddField("row", GetIndexOfPlayer(teamNum, (int) thisPlayer.deviceType, CloudGameData.gameNum));
        form.AddField("s4", "null");

        using (UnityWebRequest webRequest = UnityWebRequest.Post(CloudDataController.PushURL, form))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.LogError("An error has occurred while pushing.\n" + webRequest.error);
            }
            else
            {
                Debug.Log("Team at index: " + teamNum + (10 * (CloudGameData.gameNum + 1) + 10) + " has been succesfully left.");
            }
        }
    }

    public int GetNextTeam()
    {
        for (int i = 0; i < 4; ++i)
        {
            if (teams[i].GetComponent<Team>().players[0] == null)
            {
                return i;
            }
        }

        return -1; // Error.
    }
    public static int GetIndexOfPlayer(int teamNum, int deviceType, int gameNum)
    {
        return teamNum * 2 + deviceType + (10 * (gameNum + 1) + 10);
    }
}
