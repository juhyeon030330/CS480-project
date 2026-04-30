using UnityEngine;

public class DummyBehavior : MonoBehaviour, IDamageable
{
    public float maxHealth = 100f;
    private float currentHealth;

    void Start() => currentHealth = maxHealth;

    public void TakeDamage(DamageData data)
    {
        currentHealth -= data.amount;
        if (currentHealth <= 0) Die();
    }

    void Die() => Destroy(gameObject);
}