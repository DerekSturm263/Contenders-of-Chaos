using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.Experimental.Rendering.Universal;

public class Gem : MonoBehaviour
{
    private GamePlayerInfo info;

    public enum State
    {
        Floating, Held
    }
    public State gemState = State.Floating;

    private Float floatScript;
    private Light2D glow;
    public Transform holder;
    public int holderPlayerNum;

    private int points = 0;
    public int gemNum;

    public static int gemsLeft;

    private void Awake()
    {
        info = GamePlayerInfo.GetPlayerInfo();
        floatScript = GetComponent<Float>();
        glow = GetComponent<Light2D>();

        glow.enabled = GameController.playerInfo.deviceType == PlayerData.Device_Type.MB;
        //StartCoroutine(SendHolderInfo());
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

        transform.localScale = new Vector2(points / 3f + 0.66f, points / 3f + 0.66f);
    }

    public void Grab()
    {
        floatScript.enabled = false;
        glow.enabled = true;
        gemState = State.Held;
    }

    public void Drop()
    {
        if (GemGoal.currentGems.Contains(gameObject))
        {
            Collect();
            return;
        }

        holder = null;
        floatScript.enabled = true;
        floatScript.ResetPosition();
        gemState = State.Floating;
    }

    public void Collect()
    {
        info.Points += points;
        StartCoroutine(PushPoints());
        --gemsLeft;
        StartCoroutine(SendHolderInfo(true));

        if (gemsLeft < 1)
        {
            StartCoroutine(UIController.SendGemSeed());
            SpawnGems.LayoutGems(SpawnGems.seed);
        }

        Destroy(gameObject);
    }

    private IEnumerator SendHolderInfo(bool destroy = false)
    {
        int rowNum = GemRowNum(CloudGameData.gameNum, gemNum);
        int playerHolderNum = destroy ? -2 : -1; // -1 means no player, -2 means destroy.

        if (holder != null && !destroy)
        {
            playerHolderNum = GamePlayerInfo.playerNum;
        }

        WWWForm form = new WWWForm();
        form.AddField("groupid", "pm36");
        form.AddField("grouppw", "N3Km3yJZpM");
        form.AddField("row", rowNum);
        form.AddField("s4", playerHolderNum);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(CloudGameData.PushURL, form))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.LogError("An error has occurred while pushing.\n" + webRequest.error);
            }
        }

        if (!destroy)
        {
            StartCoroutine(SendHolderInfo());
        }
    }

    private IEnumerator GetGemHolder()
    {
        int rowNum = GemRowNum(CloudGameData.gameNum, gemNum);
        int gemStateNum = -3;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(CloudGameData.PullURL + rowNum))
        {
            yield return webRequest.SendWebRequest();

            string[] pages = rowNum.ToString().Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.LogError("An error has occurred while pulling.\n" + webRequest.error);
            }
            else
            {
                gemStateNum = int.Parse(webRequest.downloadHandler.text.Split(',')[1]);
            }
        }

        if (gemStateNum == -2) // Destroys the gem.
        {
            --gemsLeft;
            Destroy(gameObject);
        }
        else if (gemStateNum == -1 && gemState == State.Held) // Sets the gem down.
        {
            holder = null;
            floatScript.enabled = true;
            floatScript.ResetPosition();
            gemState = State.Floating;
        }
        else // Player at gemStateNum holds the gem.
        {
            floatScript.enabled = false;
            glow.enabled = true;
            gemState = State.Held;
            holderPlayerNum = gemStateNum;
        }

        StartCoroutine(GetGemHolder());
    }

    private IEnumerator PushPoints()
    {
        int rowNum = PointsRowNum(CloudGameData.gameNum, GamePlayerInfo.playerNum / 2);

        WWWForm form = new WWWForm();
        form.AddField("groupid", "pm36");
        form.AddField("grouppw", "N3Km3yJZpM");
        form.AddField("row", rowNum);
        form.AddField("s4", info.Points);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(CloudGameData.PushURL, form))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.LogError("An error has occurred while pushing.\n" + webRequest.error);
            }
        }
    }

    public static int PointsRowNum(int gameNum, int teamNum)
    {
        return 370 + (gameNum * 4) + (teamNum);
    }

    public static int GemRowNum(int gameNum, int gemNumber)
    {
        return 420 + (gameNum * 12) + (gemNumber);
    }
}
