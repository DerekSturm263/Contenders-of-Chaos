using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerCloudMovement : MonoBehaviour
{
    [HideInInspector] public LayerMask ground;

    private Animator anim;
    private SpriteRenderer sprtRndr;

    public int playerNum;
    private Vector3 targetPosition;

    public Transform currentPlatform;

    private Vector2 oldPos, newPos;
    private Vector2 moveValN;
    private Vector2 moveValL;

    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    private float currentSpeed;

    public float jumpSpeed = 15f;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        sprtRndr = GetComponent<SpriteRenderer>();

        StartCoroutine(UpdatePosition());
    }

    private void Update()
    {
        currentSpeed = Mathf.Abs(moveValL.x) > 2f ? runSpeed : walkSpeed;
        anim.SetBool("Running", currentSpeed == runSpeed);
        anim.SetFloat("Movement Speed", Mathf.Abs(moveValL.x) > 0.025f ? 1f : 0f);

        if (currentPlatform == null)
        {
            anim.SetFloat("Y Velocity", moveValL.y);
        }
        else
        {
            anim.SetFloat("Y Velocity", 0f);
        }

        anim.SetBool("Grounded", IsGrounded());

        if (moveValL.y > 0.025f)
        {
            anim.SetTrigger("Jumping");
        }
        else if (moveValL.y < -0.025f && !IsGrounded())
        {
            anim.SetFloat("Y Velocity", -1f);
        }

        if (IsGrounded())
        {
            anim.ResetTrigger("Jumping");
            anim.SetFloat("Y Velocity", -1f);
        }

        if (moveValL.x != 0)
        {
            sprtRndr.flipX = moveValL.x < 0;
        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 5f / Vector2.Distance(transform.position, targetPosition));

        newPos = transform.position;
        moveValN = newPos - oldPos;
        moveValL = Vector2.Lerp(moveValN, moveValL, Time.deltaTime * 5f);
    }

    private void LateUpdate()
    {
        oldPos = transform.position;
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
        Debug.Log(rowNum);

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
                Debug.Log(webRequest.downloadHandler.text);

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
