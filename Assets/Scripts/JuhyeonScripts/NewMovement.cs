using UnityEngine;
using UnityEngine.InputSystem;

public class NewMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 50;
    public float jumpHeight = 10;
    private Rigidbody rb;
    private float movementX, movementY;
    private bool isGrounded;

    [Header("Charge Attack")]
    public float chargePower = 20f;
    public float damageAmount = 25f;
    private bool isCharging = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; 
    }

    // --- SIMPLE MOVEMENT ---
    void OnMove(InputValue movementValue)
    {
        Vector2 v = movementValue.Get<Vector2>();
        movementX = v.x;
        movementY = v.y;
    }

    void OnJump(InputValue value)
    {
        // Simple: If you're on the ground and press jump, go up.
        if (value.isPressed && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    // --- THE CHARGE ATTACK ---
    void OnAttack(InputValue value)
    {
        if (value.isPressed)
        {
            isCharging = true;
            // Lunge forward based on where the ghost is facing
            rb.AddForce(transform.forward * chargePower, ForceMode.Impulse);
            
            // End charge state after 0.5 seconds
            Invoke(nameof(EndCharge), 0.5f);
        }
    }

    void EndCharge() => isCharging = false;

    // --- DAMAGE DETECTION ---
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) isGrounded = true;

        // If we hit something while charging...
        if (isCharging)
        {
            // Check if the object we hit has the Damage System interface
            if (collision.gameObject.TryGetComponent(out IDamageable victim))
            {
                DamageData data = new DamageData {
                    amount = damageAmount,
                    source = gameObject,
                    hitDirection = transform.forward
                };
                
                Debug.Log(victim);
                victim.TakeDamage(data);
                isCharging = false; // Stop charging after one hit
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) isGrounded = false;
    }

    void FixedUpdate()
    {
        Vector3 moveDirection = (transform.forward * movementY) + (transform.right * movementX);
        if (moveDirection.magnitude > 1) moveDirection.Normalize();

        rb.AddForce(moveDirection * speed, ForceMode.Acceleration);
    }
}