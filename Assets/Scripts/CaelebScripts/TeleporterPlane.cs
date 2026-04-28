using UnityEngine;

public class TeleportPlane : MonoBehaviour
{
    public Transform respawnPoint;
    // This script teleports the player to a specified respawn point when they enter the trigger area of the plane.
    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the player
        if (other.CompareTag("Player"))
        {
            // Teleport the player to the respawn point
            other.transform.position = respawnPoint.position;
            // Optionally, reset the player's velocity to prevent them from being launched after teleporting
            Rigidbody rb = other.GetComponent<Rigidbody>();
            // If the player has a Rigidbody component, reset its velocity
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}