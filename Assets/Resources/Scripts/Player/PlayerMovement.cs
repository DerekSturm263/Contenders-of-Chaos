using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
public class PlayerMovement : MonoBehaviour
{
    public InputActions inputActions;

    [HideInInspector] public LayerMask ground;

    private Animator anim;
    private Rigidbody2D rb2D;
    private SpriteRenderer sprtRndr;

    private Vector2 currentInputVal;

    [Header("Movement Settings")]
    public readonly float walkSpeed = 4f;
    public readonly float runSpeed = 7f;
    private float currentSpeed;

    public float jumpSpeed = 15f;

    public Transform currentPlatform;
    public GameObject overlappingObject;

    public GameObject heldItem;

    private GameObject dustParticles;

    public float diAmount = 1f;

    public Vector3 spawnPoint;

    private void Awake()
    {
        spawnPoint = transform.position;
        inputActions = new InputActions();

        anim = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        sprtRndr = GetComponent<SpriteRenderer>();

        dustParticles = Resources.Load<GameObject>("Prefabs/Dust Particles");
        Camera.main.GetComponent<CameraController>().followTrans = transform;

        StartCoroutine(SendPosition());
    }

    private void Update()
    {
        Run();
        anim.speed = currentInputVal.x != 0f ? Mathf.Abs(rb2D.velocity.x) / currentSpeed : 1f;

        rb2D.AddForce(new Vector2(currentInputVal.x * diAmount * currentSpeed * 20f, 0f));

        if (Mathf.Abs(rb2D.velocity.x) > currentSpeed)
        {
            rb2D.velocity = new Vector2(currentSpeed * Mathf.Abs(rb2D.velocity.x) / rb2D.velocity.x, rb2D.velocity.y);
        }

        if (currentInputVal.x == 0f)
        {
            rb2D.velocity = new Vector2(rb2D.velocity.x / (currentSpeed * 2f), rb2D.velocity.y);
        }

        if (currentPlatform == null)
        {
            anim.SetFloat("Y Velocity", rb2D.velocity.y);
        }
        else
        {
            anim.SetFloat("Y Velocity", 0f);
        }

        anim.SetBool("Grounded", IsGrounded());

        if (IsWallSliding(out int wallSide))
        {
            if (wallSide == (int)(currentInputVal.x / Mathf.Abs(currentInputVal.x)) && !IsGrounded() && rb2D.velocity.y < 0f)
            {
                rb2D.velocity = new Vector2(rb2D.velocity.x, currentSpeed == runSpeed ? -6f : -4f);
            }
        }

        if (transform.position.y < -40f)
        {
            Die();
        }
    }

    public void Die()
    {
        transform.position = spawnPoint;
        GamePlayerInfo.GetPlayerInfo().Points -= 2;
    }

    public void Movement(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();
        currentInputVal = input;

        if (input.x != 0)
        {
            sprtRndr.flipX = input.x < 0;
        }
        anim.SetFloat("Movement Speed", Mathf.Abs(input.x));
    }

    public void Jump()
    {
        if (IsWallSliding(out int wallSide))
        {
            if (wallSide == (int) (currentInputVal.x / Mathf.Abs(currentInputVal.x)) && !IsGrounded())
            {
                WallJump(-wallSide);
                return;
            }
        }

        if (!IsGrounded())
            return;

        GameObject newDust = Instantiate(dustParticles, transform.position - new Vector3(0f, 0.5f), Quaternion.identity);
        newDust.GetComponent<SpriteRenderer>().flipX = sprtRndr.flipX;
        rb2D.AddForce(new Vector2(0f, jumpSpeed), ForceMode2D.Impulse);
        anim.SetTrigger("Jumping");
    }

    public void WallJump(int direction)
    {
        StartCoroutine(DisableDI());
        rb2D.velocity = new Vector2(jumpSpeed * direction * 4f, jumpSpeed);
        anim.SetTrigger("Jumping");
    }

    private IEnumerator DisableDI()
    {
        diAmount = 0f;

        yield return new WaitUntil(() => rb2D.velocity.y <= 0f);

        for (float i = 0f; i < 2.5f; i += Time.deltaTime)
        {
            diAmount = 2.5f;
            yield return new WaitForEndOfFrame();
        }

        diAmount = 1f;
    }

    private void Run()
    {
        currentSpeed = Keyboard.current.leftShiftKey.isPressed ? runSpeed : walkSpeed; // TODO: Add gamepad support.
        anim.SetBool("Running", currentSpeed == runSpeed);
    }

    public void Grab()
    {
        if (overlappingObject == null)
            return;

        if (heldItem != null && heldItem.CompareTag("Gem"))
        {
            heldItem.GetComponent<Gem>().Drop();
            heldItem = null;

            return;
        }

        if (overlappingObject.CompareTag("Gem"))
        {
            Gem thisGem = overlappingObject.GetComponent<Gem>();

            if (thisGem.gemState == Gem.State.Floating && heldItem == null)
            {
                thisGem.Grab();
                heldItem = overlappingObject;
                thisGem.holder = transform;
            }
        }
        else if (overlappingObject.CompareTag("Item"))
        {
            if (heldItem == null)
            {
                ItemAction item = overlappingObject.GetComponent<ItemAction>();
                item.pickupPlayer = gameObject;

                if (item.canCarry)
                {
                    item.Grab();
                }
                else
                {
                    item.itemAction.Invoke();
                }
            }
            else if (heldItem.CompareTag("Item"))
            {
                ItemAction item = overlappingObject.GetComponent<ItemAction>();
                item.Use();
            }
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(transform.position - new Vector3(0f, 1f), new Vector2(0.5f, 0.125f), 0f, Vector2.down, 0.05f, ground);
    }

    private bool IsWallSliding(out int wallSide)
    {
        RaycastHit2D[] leftWallHit = new RaycastHit2D[1];
        RaycastHit2D[] rightWallHit = new RaycastHit2D[1];

        bool leftWall = Physics2D.BoxCast(transform.position, new Vector2(0.1f, 0.75f), 0f, Vector2.left, 0.75f, ground);
        bool rightWall = Physics2D.BoxCast(transform.position, new Vector2(0.1f, 0.75f), 0f, Vector2.right, 0.75f, ground);
        wallSide = leftWall ? -1 : rightWall ? 1 : 0;

        return (leftWall || rightWall)/* && (!leftWallHit[0].transform.CompareTag("Platform") && !rightWallHit[0].transform.CompareTag("Platform"))*/;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        currentPlatform = collision.transform;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        currentPlatform = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        overlappingObject = collision.gameObject;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        overlappingObject = collision.gameObject;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        overlappingObject = null;
    }

    private IEnumerator SendPosition()
    {
        int rowNum = TeamsUpdater.GetIndexOfPlayerPosition(GamePlayerInfo.playerNum / 2, 0, CloudGameData.gameNum);

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
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
}