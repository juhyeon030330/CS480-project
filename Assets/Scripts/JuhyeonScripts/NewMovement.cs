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

    [Header("Scripts")]
    private Attack attackScript;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        attackScript = GetComponent<Attack>();
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
        if (value.isPressed && attackScript != null)
        {
            // Flip switch ON
            attackScript.SetAttackActive(true);
            
            rb.AddForce(transform.forward * chargePower, ForceMode.Impulse);
            Invoke(nameof(EndCharge), 0.5f);
        }
    }

    void EndCharge()
    {
        // Flip switch OFF
        if(attackScript != null) attackScript.SetAttackActive(false);
    }

    // --- JUMP LOGIC ---
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) isGrounded = true;
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