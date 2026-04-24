using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerConCopy : MonoBehaviour
{
    private bool isGrounded = true;
    private bool groundJumpUsed = false;
    public int totalJumpsAllowed = 2;
    private int jumpsRemaining;
    public float jumpHeight = 10;
    public float speed = 10;
    private Rigidbody rb;
    private float movementX;
    private float movementY;
    public float attackRange = 5f;
    public GameObject hitEffectPrefab;


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
        if (value.isPressed)
        {
            // 1. Always spawn the effect in front of the ghost
            // Adjust the '2.0f' to move it further or closer to the ghost's face
            Vector3 spawnPosition = transform.position + (transform.forward * 0.7f);
            
            // Adjust the height (Y) if your ghost is floating
            spawnPosition.y += 1.0f; 

            // Spawn the effect and store it in a variable called 'effect'
            GameObject effect = Instantiate(hitEffectPrefab, spawnPosition, Quaternion.identity);

            // Tell Unity to automatically delete 'effect' after 0.5 seconds
            Destroy(effect, 0.5f);

            // 2. Check for the Boss damage separately
            GameObject boss = GameObject.FindGameObjectWithTag("Boss");
            if (boss != null)
            {
                float distance = Vector3.Distance(transform.position, boss.transform.position);
                if (distance < attackRange)
                {
                    boss.GetComponent<BossScript>().TakeDamage(10f);
                }
            }
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