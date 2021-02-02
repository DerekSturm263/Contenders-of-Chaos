using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class CloudGameData : MonoBehaviour
{
    public static bool isHosting = false;
    public static string inputRoomCode = "000000";
    public static int gameNum;

    public static string roomNum = "000000";

    private void Awake()
    {
        if (isHosting)
        {
            try
            {
                StartCoroutine(HostGame());
            }
            catch
            {
                Debug.LogError("An error has occurred whilst trying to host a game. Please try again later.");
            }
        }
        else
        {
            StartCoroutine(JoinGame(roomNum));
        }
    }

    public static CloudGameData GetCurrentCloudData()
    {
        return GameObject.FindObjectOfType<CloudGameData>();
    }

    public IEnumerator HostGame()
    {
        for (int i = 0; i < 10; ++i)
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
                    if (webRequest.downloadHandler.text.Contains("False"))
                    {
                        gameNum = i;
                        break;
                    }
                }
            }
        }

        WWWForm form = new WWWForm();
        form.AddField("groupid", "vgd21");
        form.AddField("grouppw", "foobar21");
        form.AddField("row", gameNum);
        form.AddField("s4", "True");

        using (UnityWebRequest webRequest = UnityWebRequest.Post(CloudDataController.PushURL, form))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.LogError("An error has occurred while pushing.\n" + webRequest.error);
            }
        }

        int[] numbers = new int[6];
        string code = "";

        for (int i = 0; i < numbers.Length; ++i)
        {
            numbers[i] = Random.Range(0, 10);
            code += numbers[i];
        }

        WWWForm form2 = new WWWForm();
        form2.AddField("groupid", "vgd21");
        form2.AddField("grouppw", "foobar21");
        form2.AddField("row", gameNum + 10);
        form2.AddField("s4", code);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(CloudDataController.PushURL, form2))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.LogError("An error has occurred while pushing.\n" + webRequest.error);
            }
            else
            {
                Debug.Log("New game successfully hosted at index: " + gameNum + " with code: " + code);

                roomNum = code;
                UIController.GetActiveController().UpdateRoomNumber(roomNum);
            }
        }

        StartCoroutine(TeamsUpdater.GetTeamsUpdater().UpdateTeams());
    }

    public IEnumerator JoinGame(string input)
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
                        Debug.Log("Game at index: " + gameNum + " has been succesfully joined.");
                        UIController.GetActiveController().UpdateRoomNumber(roomNum);
                        break;
                    }
                }
            }
        }

        StartCoroutine(TeamsUpdater.GetTeamsUpdater().UpdateTeams());
    }
}
