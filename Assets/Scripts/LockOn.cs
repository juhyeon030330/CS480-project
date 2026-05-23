using UnityEngine;

public class FirstPersonLockOn : MonoBehaviour
{
    [Header("Lock-On Settings")]
    public string enemyTag = "Enemy";
    public float lockOnRange = 30f;
    public float trackingSmoothSpeed = 10f;
    public float lockOnRadius = 4f; 

    [Header("Audio Settings")]
    public AudioClip lockOnSound;

    [Header("References")]
    public Transform playerBody; 

    private Transform targetEnemy;
    private bool isLockedOn = false;
    private ParticleSystem targetParticles;

    // Public property for your mouse-look script to check
    public bool IsLockedOn => isLockedOn;

    void Update()
    {
        // 1. While Left Shift is HOLDING DOWN
        if (Input.GetKey(KeyCode.LeftShift))
        {
            // If we don't have a target yet, keep scanning for one
            if (targetEnemy == null)
            {
                TryLockOn();
            }
        }
        else
        {
            // 2. If Left Shift is RELEASED, instantly clear everything
            if (isLockedOn || targetEnemy != null)
            {
                Unlock();
            }
        }


        if (isLockedOn && targetEnemy == null) 
        {
            Unlock();
        }
        if (isLockedOn && targetEnemy != null)
        {
            ExecuteLockOnRotation();
        }
    }

    private void TryLockOn()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // Sweep a thick sphere forward 
        if (Physics.SphereCast(ray, lockOnRadius, out hit, lockOnRange))
        {
            if (hit.collider.CompareTag(enemyTag))
            {
                targetEnemy = hit.transform;
                isLockedOn = true;

                // Play the sound ONCE right here when the lock-on succeeds
                if (lockOnSound != null)
                {
                    AudioSource.PlayClipAtPoint(lockOnSound, transform.position);
                }

// 1. Check the hit object or its parents for MushroomAI
MushroomAI meleeEnemy = hit.collider.GetComponentInParent<MushroomAI>();
if (meleeEnemy != null && meleeEnemy.lockOnParticles != null)
{
    targetParticles = meleeEnemy.lockOnParticles;
    targetParticles.Play();
}
else 
{
    // 2. Check the hit object or its parents for RangedMushroomAI
    RangedMushroomAI rangedEnemy = hit.collider.GetComponentInParent<RangedMushroomAI>();
    if (rangedEnemy != null && rangedEnemy.lockOnParticles != null)
    {
        targetParticles = rangedEnemy.lockOnParticles;
        targetParticles.Play();
    }
}
            }
        }
    }

    private void ExecuteLockOnRotation()
    {
        // Calculate direction to the target
        Vector3 targetDirection = targetEnemy.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Vector3 eulerAngles = targetRotation.eulerAngles;

        // --- HORIZONTAL ROTATION (Y-Axis) ---
        Quaternion bodyTargetRot = Quaternion.Euler(0, eulerAngles.y, 0);
        playerBody.rotation = Quaternion.Slerp(playerBody.rotation, bodyTargetRot, trackingSmoothSpeed * Time.deltaTime);

        // --- VERTICAL ROTATION (X-Axis) ---
        Quaternion camTargetRot = Quaternion.Euler(eulerAngles.x, playerBody.rotation.eulerAngles.y, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, camTargetRot, trackingSmoothSpeed * Time.deltaTime);
    }

    private void Unlock()
    {
        if (targetParticles != null)
        {
            targetParticles.Stop();
        }

        isLockedOn = false;
        targetEnemy = null;
        targetParticles = null;
    }
}
