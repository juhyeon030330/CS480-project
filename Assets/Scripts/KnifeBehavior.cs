using UnityEngine;

public class KnifeBehavior : MonoBehaviour
{
    private Rigidbody RB;
    
    private bool targetHit = false;

    private void Start()
    {
        RB = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision other)
    {
        // Determine if player was touched, in which case it is picked up
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerThrowingKnife>().totalKnives++;

            // Deal more damage as it is picked up if lodged in an enemy
            if (targetHit == true) {
                // ===== DAMAGE ===== (copied from FirstPersonController)
                // Build the damage package
                DamageData knifePickupDmg = new DamageData
                {
                    amount = 10f,
                    source = this.gameObject,
                    hitDirection = (other.gameObject.transform.position - transform.position).normalized
                };

                // Deal damage
                if (other.gameObject.TryGetComponent<IDamageable>(out var target))
                {
                    target.TakeDamage(knifePickupDmg);
                }
            }

            Destroy(gameObject);
        }
        // Determine if a target was hit
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Boss"))
        {
            targetHit = true;
        }
        else {
            targetHit = false;
        }

        if (targetHit == true)
        {
            // ===== DAMAGE ===== (copied from FirstPersonController)
            // Build the damage package
            DamageData knifeDmg = new DamageData
            {
                amount = 25f,
                source = this.gameObject,
                hitDirection = (other.gameObject.transform.position - transform.position).normalized
            };

            // Deal damage
            if (other.gameObject.TryGetComponent<IDamageable>(out var target))
            {
                target.TakeDamage(knifeDmg);
            }

            // Stick to enemy
            RB.isKinematic = true;
            transform.SetParent(other.gameObject.transform);
        }
    }
}
