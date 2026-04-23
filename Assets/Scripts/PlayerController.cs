using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;
    private bool isGrounded;
    private bool isDead = false;


    private InputAction moveAction;
    private InputAction jumpAction;

    void Awake()
    {
        rb   = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr   = GetComponent<SpriteRenderer>();


        moveAction = new InputAction("Move", binding: "<Keyboard>/a");
        moveAction.AddBinding("<Keyboard>/d");
        moveAction.AddCompositeBinding("1DAxis")
            .With("Negative", "<Keyboard>/leftArrow")
            .With("Positive", "<Keyboard>/rightArrow");

        jumpAction = new InputAction("Jump", binding: "<Keyboard>/space");
        jumpAction.AddBinding("<Keyboard>/upArrow");

        moveAction.Enable();
        jumpAction.Enable();
    }

    void OnDestroy()
    {
        moveAction.Disable();
        jumpAction.Disable();
    }

    void Update()
    {
        if (isDead) return;

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position, groundRadius, groundLayer);

        float h = 0f;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                h = -1f;
            else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                h = 1f;
        }

        rb.linearVelocity = new Vector2(h * moveSpeed, rb.linearVelocity.y);

        if (h > 0.01f)       sr.flipX = false;
        else if (h < -0.01f) sr.flipX = true;

        bool jumpPressed = Keyboard.current != null &&
            (Keyboard.current.spaceKey.wasPressedThisFrame ||
             Keyboard.current.upArrowKey.wasPressedThisFrame);

        if (jumpPressed && isGrounded)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        anim.SetFloat("Speed",      Mathf.Abs(h));
        anim.SetBool ("IsGrounded", isGrounded);
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        anim.SetBool("IsDead", true);
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        GetComponent<Collider2D>().enabled = false;
        Invoke(nameof(TriggerGameOver), 1.5f);
    }

    void TriggerGameOver()
    {
        GameManager.Instance.GameOver();
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }
    }
}
