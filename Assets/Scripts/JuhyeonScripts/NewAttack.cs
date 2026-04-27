using UnityEngine;
using UnityEngine.InputSystem;

public class NewAttack : MonoBehaviour
{
    private bool isGrounded = true;
    private bool groundJumpUsed = false;
    public int totalJumpsAllowed = 2;
    private int jumpsRemaining;
    public float jumpHeight = 10;
    public float speed = 50;
    private Rigidbody rb;
    private float movementX;
    private float movementY;
    public float attackRange = 5f;
    public ParticleSystem hitEffect;
    public AudioClip attackSound;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public AudioClip DiveSound;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Boss"))
        {
            Debug.Log("hello0");
            Rigidbody enemyRb = collision.gameObject.GetComponent<Rigidbody>();

            Debug.Log(rb.linearVelocity.y);
            if (rb.linearVelocity.y >= -0.05f) return;
            if (enemyRb != null)
            {
                // 1. The Knockback
                enemyRb.AddForce(transform.forward * 500f + Vector3.up * 100f);
                
                // 2. The Sound
                AudioSource.PlayClipAtPoint(attackSound, collision.transform.position);
            }
        }
    }


    void OnAttack(InputValue value)
    {
        if (!value.isPressed) return;

        hitEffect.Play();
        AudioSource.PlayClipAtPoint(attackSound, transform.position);

        GameObject[] allBosses = GameObject.FindGameObjectsWithTag("Boss");

        foreach (GameObject bossObj in allBosses)
        {
            float distance = Vector3.Distance(transform.position, bossObj.transform.position);
            if (distance <= attackRange) bossObj.GetComponent<BossScript2>().TakeDamage(10f, transform.forward);
            // Must comment out above statement because of outdataed TakeDamage() call
        }

    }



}