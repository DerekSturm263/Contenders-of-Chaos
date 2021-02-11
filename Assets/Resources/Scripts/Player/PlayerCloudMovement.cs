using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerCloudMovement : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb2D;
    private SpriteRenderer sprtRndr;

    public int playerNum;
    private Vector3 targetPosition;
    private int playerState;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        sprtRndr = GetComponent<SpriteRenderer>();

        StartCoroutine(UpdatePosition());
        StartCoroutine(UpdateState());
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 5f / Vector2.Distance(transform.position, targetPosition));

        if (rb2D.velocity.x != 0)
        {
            sprtRndr.flipX = rb2D.velocity.x < 0;
        }

        anim.Play(playerState);
    }

    private IEnumerator UpdatePosition()
    {
        int rowNum = TeamsUpdater.GetIndexOfPlayerPosition(playerNum / 2, 0, CloudGameData.gameNum);

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
                string[] webData = webRequest.downloadHandler.text.Split(',');
                string[] pos = webData[1].Split('|');

                float x = float.Parse(pos[0]);
                float y = float.Parse(pos[1]);

                targetPosition = new Vector3(x, y, 0);
            }
        }

        StartCoroutine(UpdatePosition());
    }

    private IEnumerator UpdateState()
    {
        int rowNum = TeamsUpdater.GetIndexOfPlayerState(playerNum / 2, 0, CloudGameData.gameNum);

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
                playerState = int.Parse(webRequest.downloadHandler.text.Split(',')[1]);
            }
        }

        StartCoroutine(UpdateState());
    }
}
