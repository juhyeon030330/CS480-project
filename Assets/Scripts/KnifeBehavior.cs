using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class KnifeBehavior : MonoBehaviour
{
    private Rigidbody RB;
    private bool targetHit = false;
    private GameObject hitEnemy;
    public bool hasLanded = false;

    private void Start()
    {
        RB = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") && hasLanded == true)
        {
            other.gameObject.GetComponent<PlayerThrowingKnife>().totalKnives++;

            other.gameObject.GetComponent<PlayerThrowingKnife>().AddKnife();

            if (targetHit == true && hitEnemy != null)
            {
                DamageData knifePickupDmg = new DamageData
                {
                    amount = 15f,
                    source = this.gameObject,
                    hitDirection = (hitEnemy.transform.position - transform.position).normalized
                };
                IDamageable target = hitEnemy.GetComponentInParent<IDamageable>();
                if (target != null)
                {
                    target.TakeDamage(knifePickupDmg);
                }
            }
            Destroy(gameObject);
        }

        if (hasLanded == true) return;

        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Boss"))
        {
            targetHit = true;
            hitEnemy = other.gameObject;
        }
        else
        {
            targetHit = false;
        }

        if (targetHit == true)
        {
            // Parent before dealing damage so knife is in hierarchy if enemy dies
            RB.isKinematic = true;
            transform.SetParent(other.gameObject.transform);
            hasLanded = true;

            DamageData knifeDmg = new DamageData
            {
                amount = 25f,
                source = this.gameObject,
                hitDirection = (other.gameObject.transform.position - transform.position).normalized
            };
            IDamageable target = other.gameObject.GetComponentInParent<IDamageable>();
            if (target != null)
            {
                target.TakeDamage(knifeDmg);
            }
        }
        else
        {
            hasLanded = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasLanded) return;

        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Boss"))
        {
            targetHit = true;
            hitEnemy = other.gameObject;

            // Parent before dealing damage so knife is in hierarchy if enemy dies
            RB.isKinematic = true;
            transform.SetParent(other.gameObject.transform);
            hasLanded = true;

            DamageData knifeDmg = new DamageData
            {
                amount = 25f,
                source = this.gameObject,
                hitDirection = (other.gameObject.transform.position - transform.position).normalized
            };
            IDamageable target = other.gameObject.GetComponentInParent<IDamageable>();
            if (target != null)
            {
                target.TakeDamage(knifeDmg);
            }
        }
    }

    public void Detach()
    {
        transform.SetParent(null);
        RB.isKinematic = false;
        RB.linearVelocity = Vector3.zero;
    }
}