using UnityEngine;

public class Attack : MonoBehaviour 
{
    public float damageAmount = 25f;
    private bool isAttacking = false;

    public void SetAttackActive(bool active) => isAttacking = active;

    private void OnCollisionEnter(Collision collision) 
    {
        if (!isAttacking) return;
        
        if (collision.gameObject.TryGetComponent(out IDamageable victim)) 
        {
            DamageData data = new DamageData {
                amount = damageAmount,
                source = gameObject,
                hitDirection = transform.forward
            };
            victim.TakeDamage(data);
            
            isAttacking = false;
        }
    }
}