using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class CloudDataController : MonoBehaviour
{
    public const string PushURL = "http://vgdapi.basmati.org/mods4.php";
    public const string PullURL = "http://vgdapi.basmati.org/gets4.php?groupid=vgd21&row=";

    // Pushes team information such as player names and player slots from index "index".
    public static IEnumerator Push(int index, string content)
    {
        WWWForm form = new WWWForm();
        form.AddField("groupid", "vgd21");
        form.AddField("grouppw", "foobar21");
        form.AddField("row", index);
        form.AddField("s4", content);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(PushURL, form))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isDone)
            {
                Debug.Log("Data " + content + " has succesfully been uploaded to " + PushURL + ".");
            }
            else
            {
                Debug.LogError("An error has occurred while pushing.\n" + webRequest.error);
            }
        }
    }

    // Pulls team information such as player names and player slots from index "index"
    public static IEnumerator Pull(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(PullURL + uri))
        {
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isDone)
            {
                Debug.Log("Data has been succesfully pulled from " + PullURL + uri + ".");
                CloudGameData.resultFromLastPull = webRequest.downloadHandler.text;
            }
            else
            {
                Debug.LogError("An error has occurred while pulling.\n" + webRequest.error);
            }
        }
    }
}
