using UnityEngine;

public class CogSpin : MonoBehaviour
{
    [Header("--- 1. SELF ROTATION ---")]
    [Tooltip("Enables the gear to spin on its own local axis.")]
    public bool enableSelfRotation = true;
    public Vector3 rotationAxis = Vector3.up; // Use Vector3.forward for 2D, Vector3.up for 3D ProBuilder
    public float rotationSpeed = 50f;

    [Header("--- 2. FLOATING / ELEVATOR ---")]
    [Tooltip("Enables smooth up-and-down or side-to-side hovering.")]
    public bool enableFloating = false;
    public Vector3 floatDirection = Vector3.up;
    public float floatDistance = 1.0f;
    public float floatSpeed = 2.0f;

    [Header("--- 3. ORBITING (Around a Point) ---")]
    [Tooltip("Enables the gear to circle around another object or specific point.")]
    public bool enableOrbiting = false;
    public Transform orbitTarget; // Drag a central shaft or empty GameObject here
    public Vector3 orbitAxis = Vector3.up;
    public float orbitSpeed = 30f;

    [Header("--- 4. PULSING / SCALING ---")]
    [Tooltip("Enables the gear to breathe/pulse in size (great for glass/magical effects).")]
    public bool enablePulsing = false;
    public float pulseAmount = 0.1f; // How much it expands/shrinks
    public float pulseSpeed = 3.0f;

    // Private tracking variables
    private Vector3 startPosition;
    private Vector3 startScale;
    private float floatTimer;
    private float pulseTimer;

    void Start()
    {
        // Store initial positions so calculations stay anchored
        startPosition = transform.position;
        startScale = transform.localScale;
        
        // Randomize timers slightly so multiple gears don't move in perfect, creepy unison
        floatTimer = Random.Range(0f, 10f);
        pulseTimer = Random.Range(0f, 10f);
    }

    void Update()
    {
        // 1. Handle Floating (Calculated first to establish base position if not orbiting)
        Vector3 currentTargetPosition = startPosition;
        if (enableFloating)
        {
            floatTimer += Time.deltaTime * floatSpeed;
            // Math.Sin creates a smooth wave moving back and forth between -1 and 1
            float wave = Mathf.Sin(floatTimer); 
            currentTargetPosition += floatDirection.normalized * (wave * floatDistance);
            
            // If we aren't orbiting, apply the floating position directly
            if (!enableOrbiting)
            {
                transform.position = currentTargetPosition;
            }
        }
        else if (!enableOrbiting)
        {
            // Reset to start position if floating is turned off mid-game
            transform.position = startPosition;
        }

        // 2. Handle Orbiting
        if (enableOrbiting && orbitTarget != null)
        {
            // Rotate the position around the target
            transform.RotateAround(orbitTarget.position, orbitAxis, orbitSpeed * Time.deltaTime);
            
            // If floating is ALSO enabled, layer the float offset onto the orbit position
            if (enableFloating)
            {
                floatTimer += Time.deltaTime * floatSpeed;
                float wave = Mathf.Sin(floatTimer);
                transform.position += floatDirection.normalized * (wave * floatDistance * Time.deltaTime);
            }
            
            // Re-sync startPosition to prevent snapping if orbiting is disabled
            startPosition = transform.position;
        }

        // 3. Handle Self Rotation
        if (enableSelfRotation)
        {
            transform.Rotate(rotationAxis.normalized * rotationSpeed * Time.deltaTime, Space.Self);
        }

        // 4. Handle Pulsing (Scaling)
        if (enablePulsing)
        {
            pulseTimer += Time.deltaTime * pulseSpeed;
            float wave = Mathf.Sin(pulseTimer);
            transform.localScale = startScale + Vector3.one * (wave * pulseAmount);
        }
        else
        {
            transform.localScale = startScale;
        }
    }
}
