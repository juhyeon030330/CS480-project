// CHANGE LOG
// 
// CHANGES || version VERSION
//
// "Enable/Disable Headbob, Changed look rotations - should result in reduced camera jitters" || version 1.0.1

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
    using UnityEditor;
    using System.Net;
#endif

public class FirstPersonController : MonoBehaviour
{
    private Rigidbody rb;

    #region Camera Movement Variables

    public Camera playerCamera;

    public float fov = 60f;
    public bool invertCamera = false;
    public bool cameraCanMove = true;
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 50f;

    // Crosshair
    public bool lockCursor = true;
    public bool crosshair = true;
    public Sprite crosshairImage;
    public Color crosshairColor = Color.white;

    // Internal Variables
    private float yaw = 0.0f;
    private float pitch = 0.0f;
    private Image crosshairObject;

    #region Camera Zoom Variables

    public bool enableZoom = true;
    public bool holdToZoom = false;
    public KeyCode zoomKey = KeyCode.Mouse1;
    public float zoomFOV = 30f;
    public float zoomStepTime = 5f;

    // Internal Variables
    private bool isZoomed = false;

    #endregion
    #endregion

    #region Movement Variables

    public bool playerCanMove = true;
    public float walkSpeed = 5f;
    public float maxVelocityChange = 5f;

    // Internal Variables
    private bool isWalking = false;

    #region Sprint

    public bool enableSprint = true;
    public bool unlimitedSprint = false;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public float sprintSpeed = 7f;
    public float sprintDuration = 5f;
    public float sprintCooldown = .5f;
    public float sprintFOV = 80f;
    public float sprintFOVStepTime = 10f;

    // Sprint Bar
    public bool useSprintBar = true;
    public bool hideBarWhenFull = true;
    public Image sprintBarBG;
    public Image sprintBar;
    public float sprintBarWidthPercent = .3f;
    public float sprintBarHeightPercent = .015f;

    // Internal Variables
    private CanvasGroup sprintBarCG;
    private bool isSprinting = false;
    private float sprintRemaining;
    private float sprintBarWidth;
    private float sprintBarHeight;
    private bool isSprintCooldown = false;
    private float sprintCooldownReset;

    #endregion

    #region Jump

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

    #region Crouch

    public bool enableCrouch = true;
    public bool holdToCrouch = true;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public float crouchHeight = .75f;
    public float speedReduction = .5f;

    // Internal Variables
    private bool isCrouched = false;
    private Vector3 originalScale;

    #endregion
    #endregion

    #region Head Bob

    public bool enableHeadBob = true;
    public Transform joint;
    public float bobSpeed = 10f;
    public Vector3 bobAmount = new Vector3(.15f, .05f, 0f);

    // Internal Variables
    private Vector3 jointOriginalPos;
    private float timer = 0;

    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        crosshairObject = GetComponentInChildren<Image>();

        // Set internal variables
        playerCamera.fieldOfView = fov;
        originalScale = transform.localScale;
        jointOriginalPos = joint.localPosition;

        if (!unlimitedSprint)
        {
            sprintRemaining = sprintDuration;
            sprintCooldownReset = sprintCooldown;
        }
    }

    void Start()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (crosshair)
        {
            crosshairObject.sprite = crosshairImage;
            crosshairObject.color = crosshairColor;
        }
        else
        {
            crosshairObject.gameObject.SetActive(false);
        }

        #region Sprint Bar

        sprintBarCG = GetComponentInChildren<CanvasGroup>();

        if (useSprintBar)
        {
            sprintBarBG.gameObject.SetActive(true);
            sprintBar.gameObject.SetActive(true);

            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            sprintBarWidth = screenWidth * sprintBarWidthPercent;
            sprintBarHeight = screenHeight * sprintBarHeightPercent;

            sprintBarBG.rectTransform.sizeDelta = new Vector3(sprintBarWidth, sprintBarHeight, 0f);
            sprintBar.rectTransform.sizeDelta = new Vector3(sprintBarWidth - 2, sprintBarHeight - 2, 0f);

            if (hideBarWhenFull)
            {
                sprintBarCG.alpha = 0;
            }
        }
        else
        {
            sprintBarBG.gameObject.SetActive(false);
            sprintBar.gameObject.SetActive(false);
        }

        airControlCap = new Vector3(0.0f, 0.0f, 0.0f);

        #endregion
    }

    float camRotation;

    private void Update()
    {
        #region Camera

        // Control camera movement
        if (cameraCanMove)
        {
            yaw = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * mouseSensitivity;

            if (!invertCamera)
            {
                pitch -= mouseSensitivity * Input.GetAxis("Mouse Y");
            }
            else
            {
                // Inverted Y
                pitch += mouseSensitivity * Input.GetAxis("Mouse Y");
            }

            // Clamp pitch between lookAngle
            pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);

            transform.localEulerAngles = new Vector3(0, yaw, 0);
            playerCamera.transform.localEulerAngles = new Vector3(pitch, 0, 0);
        }

        #region Camera Zoom

        // Unused
        if (enableZoom)
        {
            // Changes isZoomed when key is pressed
            // Behavior for toogle zoom
            if (Input.GetKeyDown(zoomKey) && !holdToZoom && !isSprinting)
            {
                if (!isZoomed)
                {
                    isZoomed = true;
                }
                else
                {
                    isZoomed = false;
                }
            }

            // Changes isZoomed when key is pressed
            // Behavior for hold to zoom
            if (holdToZoom && !isSprinting)
            {
                if (Input.GetKeyDown(zoomKey))
                {
                    isZoomed = true;
                }
                else if (Input.GetKeyUp(zoomKey))
                {
                    isZoomed = false;
                }
            }

            // Lerps camera.fieldOfView to allow for a smooth transistion
            if (isZoomed)
            {
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, zoomFOV, zoomStepTime * Time.deltaTime);
            }
            else if (!isZoomed && !isSprinting)
            {
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, fov, zoomStepTime * Time.deltaTime);
            }
        }

        #endregion
        #endregion

        #region Sprint

        // Unused
        if (enableSprint)
        {
            if (isSprinting)
            {
                isZoomed = false;
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, sprintFOV, sprintFOVStepTime * Time.deltaTime);

                // Drain sprint remaining while sprinting
                if (!unlimitedSprint)
                {
                    sprintRemaining -= 1 * Time.deltaTime;
                    if (sprintRemaining <= 0)
                    {
                        isSprinting = false;
                        isSprintCooldown = true;
                    }
                }
            }
            else
            {
                // Regain sprint while not sprinting
                sprintRemaining = Mathf.Clamp(sprintRemaining += 1 * Time.deltaTime, 0, sprintDuration);
            }

            // Handles sprint cooldown 
            // When sprint remaining == 0 stops sprint ability until hitting cooldown
            if (isSprintCooldown)
            {
                sprintCooldown -= 1 * Time.deltaTime;
                if (sprintCooldown <= 0)
                {
                    isSprintCooldown = false;
                }
            }
            else
            {
                sprintCooldown = sprintCooldownReset;
            }

            // Handles sprintBar 
            if (useSprintBar && !unlimitedSprint)
            {
                float sprintRemainingPercent = sprintRemaining / sprintDuration;
                sprintBar.transform.localScale = new Vector3(sprintRemainingPercent, 1f, 1f);
            }
        }

        #endregion

        #region Jump

        // Gets input and calls jump method
        if (enableJump && Input.GetKeyDown(jumpKey) && coyoteTimer > 0.0f)
        {
            Jump();
        }

        else if (enableJump && Input.GetKeyDown(jumpKey) && !isDiving && !isPouncing && coyoteTimer <= 0.0f)
        {
            Dive();
        }

        if (isGrounded)
        {
            // being grounded cancels dives and pounces, and resets combos.
            isDiving = false;
            isPouncing = false;
            lastEnemyThisCombo = null;
        }

        #endregion

        #region Crouch

        // Unused
        if (enableCrouch)
        {
            if (Input.GetKeyDown(crouchKey) && !holdToCrouch)
            {
                Crouch();
            }

            if (Input.GetKeyDown(crouchKey) && holdToCrouch)
            {
                isCrouched = false;
                Crouch();
            }
            else if (Input.GetKeyUp(crouchKey) && holdToCrouch)
            {
                isCrouched = true;
                Crouch();
            }
        }

        #endregion

        CheckGround();

        // Unused
        if (enableHeadBob)
        {
            HeadBob();
        }
    }

    void FixedUpdate()
    {
        #region Movement

        if (playerCanMove)
        {
            // Calculate how fast we should be moving
            Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            // Checks if player is walking and isGrounded
            // Will allow head bob
            if (targetVelocity.x != 0 || targetVelocity.z != 0 && isGrounded)
            {
                isWalking = true;
            }
            else
            {
                isWalking = false;
            }

            // All movement calculations shile sprint is active
            // Unused
            if (enableSprint && Input.GetKey(sprintKey) && sprintRemaining > 0f && !isSprintCooldown)
            {
                targetVelocity = transform.TransformDirection(targetVelocity) * sprintSpeed;

                // Apply a force that attempts to reach our target velocity
                Vector3 velocity = rb.linearVelocity;
                Vector3 velocityChange = (targetVelocity - velocity);
                velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                velocityChange.y = 0;

                // Player is only moving when valocity change != 0
                // Makes sure fov change only happens during movement
                if (velocityChange.x != 0 || velocityChange.z != 0)
                {
                    isSprinting = true;

                    if (isCrouched)
                    {
                        Crouch();
                    }

                    if (hideBarWhenFull && !unlimitedSprint)
                    {
                        sprintBarCG.alpha += 5 * Time.deltaTime;
                    }
                }

                rb.AddForce(velocityChange, ForceMode.VelocityChange);
            }
            // All movement calculations while walking
            else
            {
                isSprinting = false;

                if (hideBarWhenFull && sprintRemaining == sprintDuration)
                {
                    sprintBarCG.alpha -= 3 * Time.deltaTime;
                }



                // // Apply a force that attempts to reach our target velocity
                Vector3 velocity = rb.linearVelocity;
                // Vector3 velocityChange = (targetVelocity - velocity);
                // velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                // velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                // velocityChange.y = 0;

                // air acceleration is gradual; ground acceleration is instant
                if (!isGrounded)
                {
                    // TODO: Move this to its own function
                    // In essence, while airborn, our player can add a maximum speed to their trajectory.
                    // What we do here is we determine the direction to apply acceleration, ensuring that
                    // the player isn't exceeding this max-speed-vector circle.
                    targetVelocity = transform.TransformDirection(targetVelocity) * airAcceleration * Time.deltaTime;   // the force the player wants to apply
                    Vector3 moveVec = new Vector3(airControlCap.x, airControlCap.y, airControlCap.z);                   // (for now) the current spot on the air-control-circle we are
                    if ((targetVelocity + airControlCap).sqrMagnitude > maxAirControl * maxAirControl)                  // if the result of just adding the force the player wants to apply to the air-control-cap vector
                                                                                                                        // would leave that inside of the circle, we do that;
                                                                                                                        // otherwise we clamp to the circle.
                                                                                                                        // uses sqrMagnitudue to save on unnecessary sqrt calculations.
                    {
                        // clamp air control to the circle
                        airControlCap = (targetVelocity + airControlCap) / (targetVelocity + airControlCap).magnitude * maxAirControl;
                    }
                    else
                    {
                        // add it straight up
                        airControlCap = targetVelocity + airControlCap;
                    }
                    // this is slightly obtuse but we *first* update our position inside of the air control circle,
                    // and *then* use the resulting position in the air control circle to accelerate the player.
                    moveVec = airControlCap - moveVec;
                    rb.AddForce(moveVec, ForceMode.VelocityChange);
                }
                else
                {
                    // when grounded, we set linearvelocity (makes grounded movement feel more snappy)
                    targetVelocity = transform.TransformDirection(targetVelocity) * walkSpeed;
                    rb.linearVelocity = new Vector3(targetVelocity.x, velocity.y, targetVelocity.z);
                    ResetAirControl();
                }
            }
        }

        if (isGrounded)
        {
            coyoteTimer = maxCoyoteTime;
        }
        else if (coyoteTimer > 0.0f)
        {
            coyoteTimer -= Time.deltaTime;
        }
        #endregion
    }

    // Sets isGrounded based on a raycast sent straight down from the player object
    private void CheckGround()
    {
        // TODO: Diving directly on top of enemies causes the player to believe that they're grounded, which cancels the dive prematurely.
        Vector3 origin = new Vector3(transform.position.x, transform.position.y - (transform.localScale.y * .5f), transform.position.z);
        Vector3 direction = transform.TransformDirection(Vector3.down);
        float distance = .75f;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
        {
            Debug.DrawRay(origin, direction * distance, Color.red);
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void Jump()
    {
        // Adds force to the player rigidbody to jump
        // There's a period of time (maxCoyoteTime) that a player can jump after a player stops
        // being grounded for a reason other than player input (such as walking off of a ledge)
                                                // coyote time is not an original name; this is a common term for it.
                                                // Named for Wile E. Coyote, specifically the skit where he runs off
                                                // of a cliff, and doesn't start falling for a second, letting him
                                                // turn to the camera and hold up a sign like "help!"
        if (coyoteTimer > 0.0f)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpPower, rb.linearVelocity.z);
            isGrounded = false;
            coyoteTimer = 0.0f;
        }

        // When crouched and using toggle system, will uncrouch for a jump
        if (isCrouched && !holdToCrouch)
        {
            Crouch();
        }
    }

    private void Dive()
    {
        Vector3 cam_angle = Vector3.Normalize(playerCamera.transform.forward);

        // Currently, pouncing and diving are different states even though they do the same thing
        // TODO: Add dive roll, etc.
        if (cam_angle.y > 0.0f)
        {
            // pounce
            rb.linearVelocity = divePower * cam_angle;
            isPouncing = true;
        }
        else
        {
            // dive
            rb.linearVelocity = divePower * cam_angle;
            isDiving = true;
        }
    }

    // Unused.
    private void Crouch()
    {
        // Stands player up to full height
        // Brings walkSpeed back up to original speed
        if (isCrouched)
        {
            transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
            walkSpeed /= speedReduction;

            isCrouched = false;
        }
        // Crouches player down to set height
        // Reduces walkSpeed
        else
        {
            transform.localScale = new Vector3(originalScale.x, crouchHeight, originalScale.z);
            walkSpeed *= speedReduction;

            isCrouched = true;
        }
    }

    // Unused.
    private void HeadBob()
    {
        if (isWalking)
        {
            // Calculates HeadBob speed during sprint
            if (isSprinting)
            {
                timer += Time.deltaTime * (bobSpeed + sprintSpeed);
            }
            // Calculates HeadBob speed during crouched movement
            else if (isCrouched)
            {
                timer += Time.deltaTime * (bobSpeed * speedReduction);
            }
            // Calculates HeadBob speed during walking
            else
            {
                timer += Time.deltaTime * bobSpeed;
            }
            // Applies HeadBob movement
            joint.localPosition = new Vector3(jointOriginalPos.x + Mathf.Sin(timer) * bobAmount.x, jointOriginalPos.y + Mathf.Sin(timer) * bobAmount.y, jointOriginalPos.z + Mathf.Sin(timer) * bobAmount.z);
        }
        else
        {
            // Resets when play stops moving
            timer = 0;
            joint.localPosition = new Vector3(Mathf.Lerp(joint.localPosition.x, jointOriginalPos.x, Time.deltaTime * bobSpeed), Mathf.Lerp(joint.localPosition.y, jointOriginalPos.y, Time.deltaTime * bobSpeed), Mathf.Lerp(joint.localPosition.z, jointOriginalPos.z, Time.deltaTime * bobSpeed));
        }
    }

    private void ResetAirControl()
    {
        airControlCap.x = 0;
        airControlCap.y = 0;
        airControlCap.z = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        // dive bounce up
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


            // Reset the air control (See fixedUpdate for more info on air control)
            ResetAirControl();


            Vector3 dirToEnemy = other.gameObject.transform.position - rb.position; // Determine the direction to the enemy
            dirToEnemy.y = 0;                                                       // Lock the vector to the x-z plane
            dirToEnemy.Normalize();                                                 // normalize, set its length to be equal to public float onHitPopBack
            dirToEnemy *= -onHitPopBack;
            if (lastEnemyThisCombo != other.gameObject)                             // if our enemy isn't the one we comboed off last, we can continue
            {
                isDiving = false;
                isPouncing = false;
                dirToEnemy.y = onHitPopUp;
                lastEnemyThisCombo = other.gameObject;
            }
            else                                                                    // otherwise make backwards trajectory flatter and spike the player down; do not refresh dive
            {
                dirToEnemy *= 2;
                dirToEnemy.y = 0;
            }
            rb.linearVelocity = dirToEnemy;                                         // set our velocity to the resulting vector
            
        }
    }
}



// Custom Editor
