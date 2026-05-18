using System.Collections;
using UnityEngine;

public class MushroomAI : MonoBehaviour
{
    public Transform player;
    public AudioClip chargeSound;
    public AudioClip headbuttSound;
    private AudioSource audioSource;
    
    [Header("Ranges")]
    public float detectionRange = 10f; 
    public float attackRange = 2f;     

    [Header("Movement Settings")]
    public float moveSpeed = 3f;

    [Header("Particle Setup")]
    public ParticleSystem chargeParticles; 
    public ParticleSystem lockOnParticles; 

    public bool isThrusting { get; private set; } 

    private Animator anim;
    private bool isAttacking = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void FixedUpdate() // Keeping it in FixedUpdate for consistency!
    {
        if (player == null || isAttacking) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 1. ATTACK
        if (distanceToPlayer <= attackRange)
        {
            if (anim != null) anim.SetBool("isMoving", false);
            StartCoroutine(PerformHeadbuttSequence());
        }
        // 2. CHASE
        else if (distanceToPlayer <= detectionRange)
        {
            if (anim != null) anim.SetBool("isMoving", true);

            // Look directly at player (flat on the Y axis)
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
            
            // Move directly forward
            transform.Translate(Vector3.forward * moveSpeed * Time.fixedDeltaTime);
        }
        // 3. IDLE
        else
        {
            if (anim != null) anim.SetBool("isMoving", false);
        }
    }

    IEnumerator PerformHeadbuttSequence()
    {
        isAttacking = true;


        // PHASE 1: ENERGY CHARGE
        // transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        if (chargeParticles != null) chargeParticles.Play();
        audioSource.clip = chargeSound;
        audioSource.Play();


        float timer = 0f;
        while (timer < 2f)
        {
            Vector3 targetDir = new Vector3(player.position.x, transform.position.y, player.position.z) - transform.position;
            if (targetDir != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1f * Time.deltaTime);
            }


            transform.Translate(Vector3.back * (moveSpeed * 0.6f) * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
        if (chargeParticles != null) chargeParticles.Stop();

        // PHASE 2: THE HEADBUTT
        isThrusting = true;
        audioSource.clip = headbuttSound;
        audioSource.Play();       
        timer = 0f;
        while (timer < 0.15f)
        {
            transform.Translate(Vector3.forward * (moveSpeed * 12f) * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
        isThrusting = false;

        // PHASE 3: STEP BACK
        timer = 0f;
        while (timer < 0.4f)
        {
            transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < 3; i++)
        {
            // 1. Calculate a random backward direction (-45 to +45 degrees deviation)
            float randomAngle = Random.Range(-90f, 90f);
            Quaternion randomRotation = Quaternion.Euler(0, randomAngle, 0);
            Vector3 randomBackwardDir = randomRotation * Vector3.back;

            // 2. Move Phase
            float moveTimer = 0f;
            while (moveTimer < 0.2f)
            {
                // Use your smooth rotation logic here if you want it to keep facing the player
                transform.Translate(randomBackwardDir * 3f * moveSpeed * Time.deltaTime);
                moveTimer += Time.deltaTime;
                yield return null;
            }
            // 3. Pause Phase
            // yield return new WaitForSeconds(1f);
        }


        isAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}