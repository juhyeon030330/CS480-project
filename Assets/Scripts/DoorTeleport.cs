using UnityEngine;

public class DoorTeleport : MonoBehaviour
{
    public Transform spawnPoint;

    public GameObject barSound;

    public bool entrance;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (entrance == true)
                barSound.SetActive(true);
            else
                barSound.SetActive(false);

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