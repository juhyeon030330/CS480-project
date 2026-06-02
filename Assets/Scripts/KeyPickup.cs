using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    [Header("Optional Effects")]
    public AudioSource pickupSound;
    public GameObject pickupEffect;

    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (collected)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            collected = true;

            if (pickupSound != null)
            {
                pickupSound.Play();
            }

            if (pickupEffect != null)
            {
                Instantiate(pickupEffect, transform.position, transform.rotation);
            }

            // This removes the key from the scene.
            // Your end-level script will then count one less key.
            Destroy(gameObject);
        }
    }
}