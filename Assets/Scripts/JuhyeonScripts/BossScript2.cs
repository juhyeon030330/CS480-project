using UnityEngine;

public class BossScript2 : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    public Transform player;
    public float moveSpeed = 2f;
    private Rigidbody rb;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody>();
    }

    public void TakeDamage(float data, Vector3 d)
    {
        currentHealth -= data;
        Debug.Log("Boss Health: " + currentHealth);
        Vector3 forceDirection = d * 500f + Vector3.up * 100f;
        rb.AddForce(forceDirection);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Boss Defeated!");
        Destroy(gameObject);
    }

    void FixedUpdate() 
    {
        if (player != null)
        {
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0;
            rb.MovePosition(transform.position + transform.forward * moveSpeed * Time.fixedDeltaTime);
        }
    }

}
