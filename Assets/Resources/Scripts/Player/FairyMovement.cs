using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class FairyMovement : MonoBehaviour
{
    public InputActions inputActions;
    private Camera cam;

    private Animator anim;
    private Rigidbody2D rb2D;
    private SpriteRenderer sprtRndr;

    private Vector2 screenTapStartPos;
    private Vector2 screenTapCurrentPos;
    private Vector2 screenTapDeltaPos;

    public float minJoystickDist, maxJoystickDist;
    public float speed;

    private void Awake()
    {
        inputActions = new InputActions();

        anim = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        sprtRndr = GetComponent<SpriteRenderer>();

        Camera.main.GetComponent<CameraController>().followTrans = transform;

        StartCoroutine(SendPosition());
    }

    public void TapStart(InputAction.CallbackContext ctx)
    {
        screenTapStartPos = ctx.ReadValue<Vector2>();
    }

    public void Move(InputAction.CallbackContext ctx)
    {
        screenTapCurrentPos = ctx.ReadValue<Vector2>();
        screenTapDeltaPos = screenTapCurrentPos - screenTapStartPos;

        rb2D.AddForce(screenTapDeltaPos * speed);

        if (screenTapDeltaPos.x != 0)
        {
            sprtRndr.flipX = rb2D.velocity.x < 0;
        }
    }

    private IEnumerator SendPosition()
    {
        int rowNum = TeamsUpdater.GetIndexOfPlayerPosition(GamePlayerInfo.playerNum / 2, 1, CloudGameData.gameNum);

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

        StartCoroutine(SendPosition());
    }
}
