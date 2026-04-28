using UnityEngine;


public class Attack : MonoBehaviour 
{
    public float damageAmount = 25f;
    public bool isAttacking { get; private set; }
    public AudioClip attackSound;
    public ParticleSystem myEffect;
    

    public void SetAttackActive(bool active) => isAttacking = active;

    private void OnCollisionStay(Collision collision) 
    {
        if (!isAttacking) return;

        if (collision.gameObject.TryGetComponent(out IDamageable victim))
        {

            myEffect.Play();

            // damage enemy
            DamageData data = new DamageData {
                amount = damageAmount,
                source = gameObject,
                hitDirection = transform.forward
            };
            victim.TakeDamage(data);   
            isAttacking = false;
            
            // hit sound
            if (attackSound != null)
            {
                AudioSource.PlayClipAtPoint(attackSound, transform.position);
            } 

        }
    }

}