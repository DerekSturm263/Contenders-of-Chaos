using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Gem : MonoBehaviour
{
    public enum State
    {
        Floating, Held
    }
    public State gemState = State.Floating;

    private Float floatScript;
    public Transform holder;

    private int points = 0;
    public int gemNum;

    private void Awake()
    {
        floatScript = GetComponent<Float>();
        //StartCoroutine(UpdatePosition());
    }

    private void Update()
    {
        if (gemState == State.Held)
        {
            transform.position = holder.position + new Vector3(0f, 0.5f);
        }
    }

    public void SetWorth(int worth)
    {
        Debug.Log(worth);

        if (worth < 5)
        {
            points = 1;
        }
        else if (worth < 7)
        {
            points = 3;
        }
        else
        {
            points = 5;
        }

        transform.localScale = new Vector2(points / 3f + 0.33f, points / 3f + 0.33f);
    }

    public void Grab()
    {
        floatScript.enabled = false;
        gemState = State.Held;
    }

    public void Drop()
    {
        floatScript.enabled = true;
        floatScript.ResetPosition();
        gemState = State.Floating;
    }

    public void Collect()
    {
        GamePlayerInfo.points += points;
        Destroy(gameObject);
    }

    private IEnumerator UpdatePosition()
    {
        int rowNum = -1; // Set row num based on gem num.

        WWWForm form = new WWWForm();
        form.AddField("groupid", "pm36");
        form.AddField("grouppw", "N3Km3yJZpM");
        form.AddField("row", rowNum);
        form.AddField("s4", transform.position.x.ToString() + "|" + transform.position.y.ToString());

        using (UnityWebRequest webRequest = UnityWebRequest.Post(CloudGameData.PushURL, form))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.LogError("An error has occurred while pushing.\n" + webRequest.error);
            }
        }

        StartCoroutine(UpdatePosition());
    }
}
