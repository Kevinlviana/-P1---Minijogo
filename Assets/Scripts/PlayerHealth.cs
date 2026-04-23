using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public float stompThreshold = 0.1f;

    private int currentHealth;
    private PlayerController controller;
    private Rigidbody2D rb;

    void Start()
    {
        currentHealth = maxHealth;
        controller = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        UIManager.Instance.UpdateHearts(currentHealth, maxHealth);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            bool isFallingDown = rb.linearVelocity.y < -0.1f;
            bool isAboveEnemy = transform.position.y >
                                 col.transform.position.y + stompThreshold;

            if (isFallingDown && isAboveEnemy)
            {

                col.enabled = false;

                col.GetComponent<EnemyPatrol>()?.TakeHit();
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 8f);
                GameManager.Instance.EnemyKilled();
            }
            else
            {
                TakeDamage(1);
            }
        }
        else if (col.CompareTag("Coin"))
        {
            Destroy(col.gameObject);
            GameManager.Instance.CoinCollected();
        }
        else if (col.CompareTag("Goal"))
        {
            GameManager.Instance.LevelComplete();
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        UIManager.Instance.UpdateHearts(currentHealth, maxHealth);

        if (currentHealth <= 0)
            controller.Die();
    }
}