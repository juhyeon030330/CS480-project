using UnityEngine;
using UnityEngine.InputSystem;

public class NewMovement : MonoBehaviour
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
    public GameObject hitEffectPrefab;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        jumpsRemaining = totalJumpsAllowed;

        // Ensure the ghost doesn't tip over physically
        rb.freezeRotation = true; 
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

        if (isGrounded || groundJumpUsed)
        {
            if (isGrounded) groundJumpUsed = true;
            
            jumpsRemaining--;
            // Keep existing horizontal velocity, only change vertical
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpHeight, rb.linearVelocity.z);
            isGrounded = false; // Manually set to false to prevent double-triggering ground jump logic
        }
    }

    void OnAttack(InputValue value)
    {
        if (value.isPressed)
        {
            // Spawn effect in front of where the camera/ghost is looking
            Vector3 spawnPosition = transform.position + (transform.forward * 0.7f) + (Vector3.up * 1.0f);
            GameObject effect = Instantiate(hitEffectPrefab, spawnPosition, transform.rotation);
            Destroy(effect, 0.5f);

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
        // KEY CHANGE: Move relative to the direction the ghost is FACING
        // transform.forward is the local "Blue Arrow" direction
        // transform.right is the local "Red Arrow" direction
        Vector3 moveDirection = (transform.forward * movementY) + (transform.right * movementX);

        // Normalize so diagonal movement isn't faster
        if (moveDirection.magnitude > 1) moveDirection.Normalize();

        rb.AddForce(moveDirection * speed, ForceMode.Acceleration);
        
        // We REMOVED the Slerp/LookRotation logic here because the MouseLook 
        // script on the camera will now handle the ghost's rotation.
    }
}