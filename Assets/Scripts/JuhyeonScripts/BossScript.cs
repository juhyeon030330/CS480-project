using UnityEngine;

public class BossScript : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    public Transform player; // Drag the Ball into this slot in the Inspector
    public float moveSpeed = 2f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log("Boss Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Boss Defeated!");
        Destroy(gameObject); // The boss disappears
    }


    void FixedUpdate() // Use FixedUpdate for physics!
    {
        if (player != null)
        {
            // 1. Still look at the player
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

            // 2. Calculate direction
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0; // Keep movement on the flat plane

            // 3. Only move if not too close
            if (Vector3.Distance(transform.position, player.position) > 2f)
            {
                // Move using physics so it respects walls
                rb.MovePosition(transform.position + direction * moveSpeed * Time.fixedDeltaTime);
            }
        }
    }
}