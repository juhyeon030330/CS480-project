using UnityEngine;

public class NewBossScript : MonoBehaviour
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

    public void TakeDamage(float damage, Vector3 d)
    {
        currentHealth -= damage;
        Debug.Log("Boss Health: " + currentHealth);
        Debug.Log(d);
        rb.AddForce(d * 500 + Vector3.up * 100);

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

    void FixedUpdate() // Use FixedUpdate for physics!
    {
        if (player != null)
        {
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0;
            if (Vector3.Distance(transform.position, player.position) > 2f)
            {
                rb.MovePosition(transform.position + direction * moveSpeed * Time.fixedDeltaTime);
            }
        }
    }

}
