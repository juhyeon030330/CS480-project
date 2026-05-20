using UnityEngine;

public class DoorTeleport : MonoBehaviour
{
    public Transform spawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterController controller = other.GetComponent<CharacterController>();

            if (controller != null)
            {
                controller.enabled = false;
                other.transform.position = spawnPoint.position;
                other.transform.rotation = spawnPoint.rotation;
                controller.enabled = true;
            }
            else
            {
                other.transform.position = spawnPoint.position;
                other.transform.rotation = spawnPoint.rotation;
            }
        }
    }
}