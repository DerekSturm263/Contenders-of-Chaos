using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerCloudMovement : MonoBehaviour
{
    [HideInInspector] public LayerMask ground;

    private Animator anim;
    private Rigidbody2D rb2D;
    private SpriteRenderer sprtRndr;

    public int playerNum;
    private Vector3 targetPosition;

    public Transform currentPlatform;

    private Vector2 oldPos, newPos;
    private Vector2 moveVal;

    private float currentSpeed;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        sprtRndr = GetComponent<SpriteRenderer>();

        StartCoroutine(UpdatePosition());
    }

    private void Update()
    {
        currentSpeed = moveVal.x * 10f;
        anim.SetFloat("Movement Speed", Mathf.Abs(moveVal.x) / moveVal.x);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 5f / Vector2.Distance(transform.position, targetPosition));
        anim.speed = moveVal.x != 0f ? Mathf.Abs(rb2D.velocity.x) / currentSpeed : 1f;

        if (currentPlatform == null)
        {
            anim.SetFloat("Y Velocity", rb2D.velocity.y);
        }
        else
        {
            anim.SetFloat("Y Velocity", 0f);
        }

        anim.SetBool("Running", currentSpeed > 2f);
        anim.SetBool("Grounded", IsGrounded());

        if (moveVal.y > 0.1f && !IsGrounded())
        {
            anim.SetTrigger("Jumping");
        }

        if (rb2D.velocity.x != 0)
        {
            sprtRndr.flipX = rb2D.velocity.x < 0;
        }

        oldPos = transform.position;
    }

    private void LateUpdate()
    {
        newPos = transform.position;
        moveVal = newPos - oldPos;
        oldPos = newPos;
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(transform.position - new Vector3(0f, 1f), new Vector2(0.5f, 0.125f), 0f, Vector2.down, 0.05f, ground);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        currentPlatform = collision.transform;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        currentPlatform = null;
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
}
