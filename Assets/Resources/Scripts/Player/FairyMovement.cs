using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class FairyMovement : MonoBehaviour
{
    public InputActions inputActions;

    private Rigidbody2D rb2D;
    private SpriteRenderer sprtRndr;

    private Vector2 screenTapStartPos;
    private Vector2 screenTapCurrentPos;
    private Vector2 screenTapDeltaPos;

    public float minJoystickDist, maxJoystickDist;
    public float speed = 10f;

    private RectTransform tapStart;
    private RectTransform tapEnd;

    private void Awake()
    {
        inputActions = new InputActions();

        rb2D = GetComponent<Rigidbody2D>();
        sprtRndr = GetComponent<SpriteRenderer>();

        Camera.main.GetComponent<CameraController>().followTrans = transform;

        tapStart = GameObject.FindGameObjectWithTag("UI Tap Start").GetComponent<RectTransform>();
        tapEnd = GameObject.FindGameObjectWithTag("UI Tap End").GetComponent<RectTransform>();

        StartCoroutine(SendPosition());
    }

    private void Update()
    {
        if (Input.touches.Length > 0)
            if (Input.touches[0].phase == UnityEngine.TouchPhase.Ended) TapRelease();
    }

    public void TapStart(InputAction.CallbackContext ctx)
    {
        tapStart.gameObject.SetActive(true);

        screenTapStartPos = ctx.ReadValue<Vector2>();
        tapStart.anchoredPosition = screenTapStartPos - new Vector2Int(Screen.width, Screen.height) / 2;
    }

    public void Move(InputAction.CallbackContext ctx)
    {
        tapEnd.gameObject.SetActive(true);

        screenTapCurrentPos = ctx.ReadValue<Vector2>();
        tapEnd.anchoredPosition = screenTapCurrentPos - new Vector2Int(Screen.width, Screen.height) / 2;

        screenTapDeltaPos = screenTapCurrentPos - screenTapStartPos;
        rb2D.velocity = screenTapDeltaPos.normalized * speed;

        if (screenTapDeltaPos.x != 0)
        {
            sprtRndr.flipX = rb2D.velocity.x < 0;
        }
    }

    public void TapRelease()
    {
        tapStart.gameObject.SetActive(false);
        tapEnd.gameObject.SetActive(false);

        rb2D.velocity = Vector2.zero;
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

    private void OnEnable()
    {
        inputActions.Enable();
        TouchSimulation.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
        TouchSimulation.Disable();
    }
}
