using UnityEngine;

public class WallScrew : MonoBehaviour, IDamageable
{
    [Header("Screw Movement Settings")]
    [SerializeField] private float travelDistance = 0.5f; 
    [SerializeField] private float smoothingSpeed = 10f; 
    
    [Header("Hit Limits")]
    [SerializeField] private int maxHits = 4;
    private int currentHits = 0;
    private bool isFullyIn = false;
    
    [Header("Constraints")]
    [SerializeField] private bool pushInwardOnly = true;  
    
    private Vector3 targetPosition;

    void Awake()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
        if (transform.position != targetPosition)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothingSpeed);
        }
    }

    public void TakeDamage(DamageData data)
    {
        // Stop accepting hits if it's already completely screwed in
        if (isFullyIn) return;

        Vector3 localZAxis = -transform.up;
        float forceDirection = Vector3.Dot(data.hitDirection.normalized, localZAxis);

        if (pushInwardOnly)
        {
            forceDirection = Mathf.Abs(forceDirection);
        }

        targetPosition += localZAxis * forceDirection * travelDistance;
        
        // Track the hits
        currentHits++;
        if (currentHits >= maxHits)
        {
            isFullyIn = true;
            TriggerGameEnding();
        }
    }

    private void TriggerGameEnding()
    {
        // Find the ending manager in the scene and start the ceremony
        EndingManager endingManager = Object.FindFirstObjectByType<EndingManager>();
        if (endingManager != null)
        {
            endingManager.StartEndingSequence();
        }
        else
        {
            Debug.LogWarning("No EndingManager found in the scene!");
        }
    }
}
