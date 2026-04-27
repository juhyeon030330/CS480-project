using UnityEngine;

public class BossScript : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    public Transform player;
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
        rb.AddForce(new Vector3(100, 1000, 100));

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


    // void FixedUpdate() // Use FixedUpdate for physics!
    // {
    //     if (player != null)
    //     {
    //         transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
    //         Vector3 direction = (player.position - transform.position).normalized;
    //         direction.y = 0;
    //         if (Vector3.Distance(transform.position, player.position) > 2f)
    //         {
    //             rb.MovePosition(transform.position + direction * moveSpeed * Time.fixedDeltaTime);
    //         }
    //     }
    // }
}