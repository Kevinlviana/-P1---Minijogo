using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public float moveSpeed = 6f;
    public float acceleration = 50f;
    public float deceleration = 40f;

    public float jumpForce = 16f;
    public float jumpHoldForce = 25f;
    public float maxJumpHoldTime = 0.2f;
    public float fallMultiplier = 3.5f;
    public float lowJumpMultiplier = 2f;

    public float coyoteTime = 0.12f;
    public float jumpBufferTime = 0.12f;

    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;

    private Vector2 moveInput;
    private bool isGrounded;
    private bool isDead = false;

    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private bool isJumping = false;
    private bool jumpHeld = false;
    private float jumpHoldCounter = 0f;

    private static readonly int AnimSpeed = Animator.StringToHash("Speed");
    private static readonly int AnimIsGrounded = Animator.StringToHash("IsGrounded");
    private static readonly int AnimIsDead = Animator.StringToHash("IsDead");

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        rb.freezeRotation = true;
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if (isDead) return;

        if (value.isPressed)
        {
            jumpBufferCounter = jumpBufferTime;
            jumpHeld = true;
        }
        else
        {
            jumpHeld = false;
        }
    }

    void Update()
    {
        if (isDead) return;

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.deltaTime;

        if (jumpBufferCounter > 0f)
            jumpBufferCounter -= Time.deltaTime;

        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            ExecuteJump();
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
        }

        if (isJumping && jumpHeld && jumpHoldCounter < maxJumpHoldTime)
        {
            rb.linearVelocity += Vector2.up * jumpHoldForce * Time.deltaTime;
            jumpHoldCounter += Time.deltaTime;
        }

        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y *
                                 (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !jumpHeld)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y *
                                 (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        if (moveInput.x > 0.01f) sr.flipX = false;
        else if (moveInput.x < -0.01f) sr.flipX = true;

        anim.SetFloat(AnimSpeed, Mathf.Abs(moveInput.x));
        anim.SetBool(AnimIsGrounded, isGrounded);
    }

    void FixedUpdate()
    {
        if (isDead) return;

        if (isGrounded) isJumping = false;

        float targetSpeed = moveInput.x * moveSpeed;
        float speedDiff = targetSpeed - rb.linearVelocity.x;
        float accelRate = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration;

        if (!isGrounded) accelRate *= 0.6f;

        rb.AddForce(Vector2.right * speedDiff * accelRate);

        rb.linearVelocity = new Vector2(
            Mathf.Clamp(rb.linearVelocity.x, -moveSpeed, moveSpeed),
            rb.linearVelocity.y);
    }

    void ExecuteJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        isJumping = true;
        jumpHoldCounter = 0f;
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        anim.SetBool(AnimIsDead, true);

        GetComponent<Collider2D>().enabled = false;
        Invoke(nameof(TriggerGameOver), 1.5f);
    }

    void TriggerGameOver()
    {
        GameManager.Instance?.GameOver();
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}