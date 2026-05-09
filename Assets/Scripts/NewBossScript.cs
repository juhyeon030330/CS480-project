using UnityEngine;

public class NewBossScript : MonoBehaviour, IDamageable
{
    public float maxHealth = 100f;
    public float currentHealth;

    public Transform player;
    public float moveSpeed = 2f;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(DamageData data)
    {
        currentHealth -= data.amount;
        Debug.Log("Boss Health: " + currentHealth);
        Vector3 forceDirection = data.hitDirection * 500f + Vector3.up * 100f;
        // Simulate physics
        transform.Translate(forceDirection * Time.deltaTime * 0.01f, Space.World);

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
            transform.Translate(Vector3.forward * moveSpeed * Time.fixedDeltaTime);
        }
    }

}
