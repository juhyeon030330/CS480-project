using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerConCopy : MonoBehaviour
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
        jumpsRemaining = totalJumpsAllowed;
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 v = movementValue.Get<Vector2>();
        movementX = v.x;
        movementY = v.y;
    }

    void OnJump(InputValue value)
    {
        if (!value.isPressed || jumpsRemaining <= 0) return;

        // the first ground jump
        if (isGrounded)
        {
            groundJumpUsed = true;
            jumpsRemaining--;
            float x = rb.linearVelocity.x;
            float z = rb.linearVelocity.z;
            rb.linearVelocity = new Vector3(x, jumpHeight, z);
        }
        // mid-air jump is only possible after ground jump
        else if (groundJumpUsed)
        {
            jumpsRemaining--;
            float x = rb.linearVelocity.x;
            float z = rb.linearVelocity.z;
            rb.linearVelocity = new Vector3(x, jumpHeight, z);
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
            if (distance <= attackRange) bossObj.GetComponent<NewBossScript>().TakeDamage(10f, transform.forward);
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            groundJumpUsed = false;
            jumpsRemaining = totalJumpsAllowed;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    void FixedUpdate()
    {
        Vector3 v3 = new Vector3(movementX, 0.0f, movementY);
        rb.AddForce(speed * v3);

        if (v3.magnitude > 0.1f)
        {
            // Calculate the direction to look at
            Quaternion targetRotation = Quaternion.LookRotation(v3);
            
            // Smoothly rotate toward that direction over time
            // Increase '10f' for faster turning, decrease for slower
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);
        }
    }

}