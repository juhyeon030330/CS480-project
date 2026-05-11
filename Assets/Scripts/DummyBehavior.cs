using UnityEngine;

public class DummyBehavior : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;
    public bool unbreakable = false;

    [Header("Knockback (No Rigidbody)")]
    public bool useKnockback = false; 
    public float knockbackDistance = 2f;
    public float knockbackDuration = 0.2f;
    private float knockbackTimer = 0f;
    private Vector3 knockbackTarget;

    [Header("AI Settings")]
    public bool useAI = false; 
    public float moveSpeed = 2f;
    public float patrolDistance = 3f;
    private Vector3 startPosition;
    private int direction = 1;

    void Awake()
    {
        currentHealth = maxHealth;
        startPosition = transform.position;
    }

    void Update()
    {
        // If we are currently being knocked back, handle that movement first
        if (useKnockback && knockbackTimer > 0)
        {
            float t = 1f - (knockbackTimer / knockbackDuration);
            transform.position = Vector3.Lerp(transform.position, knockbackTarget, t);
            knockbackTimer -= Time.deltaTime;
        }
        else if (useAI)
        {
            HandlePatrol();
        }
    }

    private void HandlePatrol()
    {
        transform.Translate(Vector3.forward * direction * moveSpeed * Time.deltaTime);

        // Check distance from start
        if (Vector3.Distance(startPosition, transform.position) > patrolDistance)
        {
            direction *= -1;
            transform.Rotate(0, 180, 0);
        }
    }

    public void TakeDamage(DamageData data)
    {
        if (unbreakable == false)
        {
            currentHealth -= data.amount;
        }

        if (useKnockback)
        {
            // Calculate a point in the distance to move toward
            knockbackTarget = transform.position + (data.hitDirection * knockbackDistance);
            knockbackTimer = knockbackDuration;
        }

        if (currentHealth <= 0) Die();
    }

    void Die() => Destroy(gameObject);
}