using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    private Rigidbody rb;
    public AudioClip DiveSound;  

    #region Camera Movement Variables
    public Camera playerCamera;
    public float fov = 60f;
    public bool invertCamera = false;
    public bool cameraCanMove = true;
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 50f;

    // Internal Variables
    private float yaw = 0.0f;
    private float pitch = 0.0f;
    #endregion

    #region Movement Variables
    public bool playerCanMove = true;
    public float walkSpeed = 5f;
    public float maxVelocityChange = 5f;

    #region Jump & Dive
    public bool enableJump = true;
    public KeyCode jumpKey = KeyCode.Space;
    public float jumpPower = 5f;
    public float divePower = 20f;
    public float maxCoyoteTime = 0.2f;
    public float maxAirControl = 5f;
    public float airAcceleration = 5f;
    public float onHitPopUp = 7f;
    public float onHitPopBack = 2f;

    // Internal Variables
    private bool isGrounded = false;
    private bool isDiving = false;
    private bool isPouncing = false;
    private float coyoteTimer;
    private Vector3 airControlCap;
    private GameObject lastEnemyThisCombo = null;
    #endregion
    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerCamera.fieldOfView = fov;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // Control camera movement
        if (cameraCanMove)
        {
            yaw = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch += (invertCamera ? 1 : -1) * (mouseSensitivity * Input.GetAxis("Mouse Y"));
            pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);

            transform.localEulerAngles = new Vector3(0, yaw, 0);
            playerCamera.transform.localEulerAngles = new Vector3(pitch, 0, 0);
        }

        // Jump and Dive Logic
        if (enableJump && Input.GetKeyDown(jumpKey))
        {
            if (coyoteTimer > 0.0f) Jump();
            else if (!isDiving && !isPouncing) {
                Dive();
                AudioSource.PlayClipAtPoint(DiveSound, transform.position);
            }
        }

        if (isGrounded)
        {
            isDiving = false;
            isPouncing = false;
            lastEnemyThisCombo = null;
        }

        CheckGround();
    }

    void FixedUpdate()
    {
        if (playerCanMove)
        {
            Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            Vector3 velocity = rb.linearVelocity;

            if (!isGrounded)
            {
                // Air Control Logic
                targetVelocity = transform.TransformDirection(targetVelocity) * airAcceleration * Time.deltaTime;
                Vector3 moveVec = airControlCap;
                
                if ((targetVelocity + airControlCap).sqrMagnitude > maxAirControl * maxAirControl)
                {
                    airControlCap = (targetVelocity + airControlCap).normalized * maxAirControl;
                }
                else
                {
                    airControlCap += targetVelocity;
                }
                
                moveVec = airControlCap - moveVec;
                rb.AddForce(moveVec, ForceMode.VelocityChange);
            }
            else
            {
                // Ground Movement Logic
                targetVelocity = transform.TransformDirection(targetVelocity) * walkSpeed;
                rb.linearVelocity = new Vector3(targetVelocity.x, velocity.y, targetVelocity.z);
                ResetAirControl();
            }
        }

        // Coyote Timer update
        if (isGrounded) coyoteTimer = maxCoyoteTime;
        else if (coyoteTimer > 0.0f) coyoteTimer -= Time.deltaTime;
    }

    private void CheckGround()
    {
        Vector3 origin = new Vector3(transform.position.x, transform.position.y - (transform.localScale.y * .5f), transform.position.z);
        Vector3 direction = Vector3.down;
        float distance = .75f;

        isGrounded = Physics.Raycast(origin, direction, distance);
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpPower, rb.linearVelocity.z);
        isGrounded = false;
        coyoteTimer = 0.0f;
    }

    private void Dive()
    {
        Vector3 cam_angle = playerCamera.transform.forward;
        rb.linearVelocity = divePower * cam_angle;

        if (cam_angle.y > 0.0f) isPouncing = true;
        else isDiving = true;
    }

    private void ResetAirControl()
    {
        airControlCap = Vector3.zero;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Dive bounce logic
        if ((other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Boss")) && (isDiving || isPouncing))
        {
            // Build the damage package
            DamageData dmg = new DamageData
            {
                amount = 25f,
                source = this.gameObject,
                hitDirection = (other.transform.position - transform.position).normalized
            };

            // Deal damage
            if (other.TryGetComponent<IDamageable>(out var target))
            {
                target.TakeDamage(dmg);
            }

            // Fix Me! Double flip!
            ResetAirControl();
            Vector3 dirToEnemy = (other.gameObject.transform.position - rb.position);
            dirToEnemy.y = 0;
            dirToEnemy = dirToEnemy.normalized * -onHitPopBack;

            if (lastEnemyThisCombo != other.gameObject)
            {
                isDiving = false;
                isPouncing = false;
                dirToEnemy.y = onHitPopUp;
                lastEnemyThisCombo = other.gameObject;
            }
            else
            {
                dirToEnemy *= 2;
            }
            rb.linearVelocity = dirToEnemy;
        }
    }
}