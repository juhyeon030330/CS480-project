using System.Collections;
using UnityEngine;

public class RangedMushroomAI : MonoBehaviour
{
    public Transform player;
    private AudioSource audioSource;
    public AudioClip shootSound;
    
    [Header("Ranges")]
    public float detectionRange = 12f; 
    public float attackRange = 7f;     
    public float fleeRange = 4f; 

    [Header("Movement Settings")]
    public float moveSpeed = 2.5f;

    [Header("Ranged Weapon Setup")]
    [Tooltip("Drag your Particle System PREFAB here from your Project assets folder.")]
    public GameObject sporePrefab; 
    [Tooltip("How far in front of the mushroom the projectile should spawn.")]
    public float spawnOffset = 1.2f;
    public float attackCooldown = 4f;
    public ParticleSystem lockOnParticles; 

    private Animator anim;
    private bool isAttacking = false; 
    private bool canShoot = true;     
    private bool isFleeing = false; // Tracks if the zigzag flee routine is running

    void Start()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // ALWAYS keep eyes on the target if within detection range (Moved to Update for smoother rotation visuals)
        if (player != null && Vector3.Distance(transform.position, player.position) <= detectionRange)
        {
            FacePlayer();
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 1. FLEE CONDITION (Overrides everything else)
        if (distanceToPlayer <= fleeRange)
        {
            // If we aren't already running the flee loop, start it
            if (!isFleeing)
            {
                StartCoroutine(FleeSequence());
            }
        }
        // 2. ATTACK CONDITION (Only if we aren't fleeing, already attacking, or cooling down)
        else if (distanceToPlayer <= attackRange && canShoot && !isAttacking && !isFleeing)
        {
            if (anim != null) anim.SetBool("isMoving", false);
            StartCoroutine(ShootSporeSequence());
        }
        // 3. CHASE CONDITION
        else if (distanceToPlayer <= detectionRange && !isAttacking && !isFleeing)
        {
            if (anim != null) anim.SetBool("isMoving", true);
            transform.Translate(Vector3.forward * moveSpeed * Time.fixedDeltaTime);
        }
        // 4. IDLE
        else
        {
            // Only turn off movement animation if we aren't locked in an attack or fleeing loop
            if (!isAttacking && !isFleeing) 
            {
                if (anim != null) anim.SetBool("isMoving", false);
            }
        }
    }

    void FacePlayer()
    {
        Vector3 lookTarget = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(lookTarget);
    }

    IEnumerator FleeSequence()
    {
        isFleeing = true;
        if (anim != null) anim.SetBool("isMoving", true);

        // Run your exact 3-step zigzag behavior
        for (int i = 0; i < 3; i++)
        {
            // 1. Calculate a random backward direction (-90 to +90 degrees deviation)
            float randomAngle = Random.Range(-90f, 90f);
            Quaternion randomRotation = Quaternion.Euler(0, randomAngle, 0);
            Vector3 randomBackwardDir = randomRotation * Vector3.back;

            // 2. Move Phase (0.2 seconds burst)
            float moveTimer = 0f;
            while (moveTimer < 0.2f)
            {
                // Moved to Time.deltaTime since it runs inside a frame-yielding Coroutine
                transform.Translate(randomBackwardDir * 3f * moveSpeed * Time.deltaTime);
                moveTimer += Time.deltaTime;
                yield return null;
            }

            // 3. Pause Phase (Commented out in your prompt, but here if you want it)
            yield return new WaitForSeconds(1f);
        }

        isFleeing = false;
    }

    IEnumerator ShootSporeSequence()
    {
        isAttacking = true;
        canShoot = false; 

        FacePlayer();

        if (anim != null) anim.SetTrigger("attack"); 

        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        Vector3 spawnPosition = transform.position + (transform.forward + transform.up) * spawnOffset;

        if (sporePrefab != null)
        {
            GameObject spawnedSpore = Instantiate(sporePrefab, spawnPosition, transform.rotation);
            Destroy(spawnedSpore, 5f);
        }

        yield return new WaitForSeconds(0.5f); 
        isAttacking = false; 

        yield return new WaitForSeconds(attackCooldown - 0.5f);
        canShoot = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, fleeRange);
        Gizmos.color = Color.magenta; 
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * spawnOffset);
    }
}