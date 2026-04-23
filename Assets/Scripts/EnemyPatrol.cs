using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public float speed = 2f;
    public float awakeRange = 6f;

    public float groundCheckDistance = 0.6f; 
    public float wallCheckDistance = 0.3f; 
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    private bool isDead = false;
    private bool isAwake = false;
    private int direction = 1;
    private Transform player;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (isDead) return;

        if (!isAwake && player != null)
        {
            if (Vector2.Distance(transform.position, player.position) <= awakeRange)
            {
                isAwake = true;
                anim.SetBool("IsAwake", true);
            }
        }
    }

    void FixedUpdate()
    {
        if (isDead || !isAwake) return;

        Vector2 groundCheckPos = (Vector2)transform.position
            + new Vector2(direction * groundCheckDistance, -0.5f);
        bool hasGroundAhead = Physics2D.OverlapCircle(groundCheckPos, 0.1f, groundLayer);

        Vector2 wallCheckPos = (Vector2)transform.position
            + new Vector2(direction * wallCheckDistance, 0f);
        bool hasWallAhead = Physics2D.OverlapCircle(wallCheckPos, 0.1f, groundLayer);

        if (!hasGroundAhead || hasWallAhead)
            direction *= -1;

        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
        sr.flipX = (direction == -1);
    }

    public void TakeHit()
    {
        if (isDead) return;
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        anim.SetBool("IsHit", true);
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 0.7f);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
            col.GetComponent<PlayerHealth>()?.TakeDamage(1);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, awakeRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(
            (Vector2)transform.position + new Vector2(direction * groundCheckDistance, -0.5f),
            0.1f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            (Vector2)transform.position + new Vector2(direction * wallCheckDistance, 0f),
            0.1f);
    }
}