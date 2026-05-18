using UnityEngine;

public class MoveForward : MonoBehaviour
{
    // States for our projectile
    private enum ProjectileState { Charging, Flying }
    private ProjectileState currentState = ProjectileState.Charging;

    [Header("Movement Settings")]
    public float speed = 10f;

    [Header("Charge Settings")]
    [Tooltip("How long the particle hovers and grows before shooting forward.")]
    public float chargeDuration = 1.2f;
    [Tooltip("The target scale multiplier at the end of the charge (e.g., 2 means twice as big).")]
    public float targetScaleMultiplier = 2f;

    private Vector3 initialScale;
    private Vector3 targetScale;
    private float chargeTimer = 0f;

    void Start()
    {
        // Save the size the projectile had when it was spawned
        initialScale = transform.localScale;
        // Calculate what the double-size should look like
        targetScale = initialScale * targetScaleMultiplier;
    }

    void Update()
    {
        switch (currentState)
        {
            case ProjectileState.Charging:
                HandleChargingState();
                break;

            case ProjectileState.Flying:
                HandleFlyingState();
                break;
        }
    }

    private void HandleChargingState()
    {
        chargeTimer += Time.deltaTime;

        // Calculate a 0 to 1 value representing our progress through the charge time
        float progress = chargeTimer / chargeDuration;

        // Smoothly scale up the particle ball from its starting size to its double size
        transform.localScale = Vector3.Lerp(initialScale, targetScale, progress);

        // Once the timer hits the duration, switch states to start flying!
        if (chargeTimer >= chargeDuration)
        {
            // Ensure it snaps perfectly to the target size
            transform.localScale = targetScale; 
            currentState = ProjectileState.Flying;
        }
    }

    private void HandleFlyingState()
    {
        // Shoot forward!
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}