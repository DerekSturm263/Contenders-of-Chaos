using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class TeamsUpdater : MonoBehaviour
{
    private GameObject[] teams = new GameObject[4];
    private PlayerData thisPlayer;

    private void Start()
    {
        thisPlayer = new PlayerData("Test Player " + Random.Range(0, 100));
        teams = GameObject.FindGameObjectsWithTag("Team");

        StartCoroutine(UpdateTeams());
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

                if (webRequest.isDone)
                {
                    if (webRequest.downloadHandler.text.Contains("p"))
                    {
                        Debug.Log("Data: p" + webRequest.downloadHandler.text + " has been succesfully pulled from database.");

                        PlayerData newPlayer = new PlayerData(webRequest.downloadHandler.text);
                        newPlayer.deviceType = PlayerData.Device_Type.PC;

                        Debug.Log(thisPlayer.name + " has joined on a " + thisPlayer.deviceType + ".");
                        teams[i / 2].GetComponent<Team>().AddPlayer(new PlayerData(webRequest.downloadHandler.text));
                    }
                }
                else
                {
                    Debug.LogError("An error has occurred while pulling.\n" + webRequest.error);
                }

                teams[i / 2].GetComponent<Team>().UpdateInfo();
            }
        }

        StartCoroutine(JoinTeam());
    }

    private IEnumerator JoinTeam()
    {
        int teamNum = GetNextTeam();
        teams[teamNum].GetComponent<Team>().AddPlayer(thisPlayer);

        WWWForm form = new WWWForm();
        form.AddField("groupid", "vgd21");
        form.AddField("grouppw", "foobar21");
        form.AddField("row", teamNum + (10 * (CloudGameData.gameNum + 1) + 10));
        form.AddField("s4", "p" + thisPlayer.name);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(CloudDataController.PushURL, form))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isDone)
            {
                Debug.Log("Data: p" + thisPlayer.name + " has succesfully been uploaded to database at index: " + (teamNum + (10 * (CloudGameData.gameNum + 1) + 10)));
            }
            else
            {
                Debug.LogError("An error has occurred while pushing.\n" + webRequest.error);
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
        form.AddField("row", teamNum + (10 * (CloudGameData.gameNum + 1) + 10));
        form.AddField("s4", "null");

        using (UnityWebRequest webRequest = UnityWebRequest.Post(CloudDataController.PushURL, form))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isDone)
            {
                Debug.Log("Data null has succesfully been uploaded to database at index: " + teamNum + (10 * (CloudGameData.gameNum + 1) + 10));
            }
            else
            {
                Debug.LogError("An error has occurred while pushing.\n" + webRequest.error);
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

        return -1;
    }
}
