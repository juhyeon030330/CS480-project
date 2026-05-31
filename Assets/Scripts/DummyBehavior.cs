using System.Collections;
using UnityEngine;

public class DummyBehavior : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private bool unbreakable = false;
    private float currentHealth;

    [Header("Animation & Flinch Settings")]
    [SerializeField] private Animator animator;
    [SerializeField] private string flinchTriggerName = "Flinch";
    [SerializeField] private float flinchDuration = 0.5f;

    [Header("Knockback Settings (No Rigidbody)")]
    [SerializeField] private bool useKnockback = true; 
    [SerializeField] private float knockbackDistance = 4f;
    private bool isFlinching = false;
    private Vector3 knockbackTarget;
    private AudioSource audioSource;

    void Awake()
    {
        currentHealth = maxHealth;
        if (animator == null) animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Smoothly interpolate position only while flinching
        if (useKnockback && isFlinching)
        {
            // Frame-rate independent smoothing toward the target position
            transform.position = Vector3.Lerp(transform.position, knockbackTarget, Time.deltaTime * (1f / flinchDuration));
        }
    }

    public void TakeDamage(DamageData data)
    {
        if (!unbreakable)
        {
            currentHealth -= data.amount;
        }

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        // Trigger Flinch and Knockback
        StartCoroutine(FlinchRoutine(data.hitDirection));
    }

    private IEnumerator FlinchRoutine(Vector3 hitDirection)
    {

        isFlinching = true;

        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop(); 
        }

        // 1. Trigger the animator parameter
        if (animator != null)
        {
            animator.SetTrigger(flinchTriggerName);
        }

        // 2. Set up the transform-based knockback destination
        if (useKnockback)
        {
            // Flatten hit direction on the Y-axis to prevent the dummy from sinking or flying
            Vector3 flatDirection = new Vector3(hitDirection.x, 0, hitDirection.z).normalized;
            knockbackTarget = transform.position + (flatDirection * knockbackDistance);
        }

        // 3. Keep the flinch state active for the designated duration
        yield return new WaitForSeconds(flinchDuration);

        isFlinching = false;
    }

    private void Die()
    {
        KnifeBehavior[] knives = GetComponentsInChildren<KnifeBehavior>();

        foreach (KnifeBehavior knife in knives)
        {
            knife.Detach();
        }

        Destroy(gameObject);
    }
}