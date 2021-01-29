using UnityEngine;

public class Movement : MonoBehaviour
{
    public LayerMask ground;

    private Animator anim;
    private Rigidbody2D rb2D;
    private SpriteRenderer sprtRndr;

    [Header("Movement Settings")]
    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    private float currentSpeed;

    public float jumpSpeed = 5f;

    public Transform currentPlatform;

    public GameObject dustParticles;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        sprtRndr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        float xInput = Input.GetAxisRaw("Horizontal");

        rb2D.velocity = new Vector3(xInput * currentSpeed, rb2D.velocity.y);
        anim.SetFloat("Movement Speed", Mathf.Abs(rb2D.velocity.x));

        anim.speed = xInput != 0f ? Mathf.Abs(rb2D.velocity.x) / currentSpeed : 1f;

        if (xInput != 0f)
        {
            sprtRndr.flipX = (rb2D.velocity.x < 0f) ? true : false;
        }

        if (Input.GetButtonDown("Jump") && IsGrounded())
            Jump();

        currentSpeed = Input.GetButton("Run") ? runSpeed : walkSpeed;
        anim.SetBool("Running", Input.GetButton("Run"));

        if (currentPlatform == null)
        {
            anim.SetFloat("Y Velocity", rb2D.velocity.y);
        }
        else
        {
            anim.SetFloat("Y Velocity", 0f);
        }

        anim.SetBool("Grounded", IsGrounded());
    }

    private void Jump()
    {
        GameObject newDust = Instantiate(dustParticles, transform.position - new Vector3(0f, 0.5f), Quaternion.identity);
        newDust.GetComponent<SpriteRenderer>().flipX = sprtRndr.flipX;
        rb2D.AddForce(new Vector2(0f, jumpSpeed), ForceMode2D.Impulse);
        anim.SetTrigger("Jumping");
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
}
