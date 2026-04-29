using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    public float maxHealth = 100f;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    // This is the implementation of the Interface
    public void TakeDamage(DamageData data)
    {
        currentHealth -= data.amount;
        Debug.Log($"{gameObject.name} was hit by {data.source.name} for {data.amount} damage!");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} died!");
        Destroy(gameObject);
    }
}