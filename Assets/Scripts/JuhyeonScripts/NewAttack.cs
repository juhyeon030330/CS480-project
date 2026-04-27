using UnityEngine;
using UnityEngine.InputSystem;

public class NewAttack : MonoBehaviour
{
    private bool isGrounded = true;
    private bool groundJumpUsed = false;
    public int totalJumpsAllowed = 2;
    private int jumpsRemaining;
    public float jumpHeight = 10;
    public float speed = 50;
    private Rigidbody rb;
    private float movementX;
    private float movementY;
    public float attackRange = 5f;
    public ParticleSystem hitEffect;
    public AudioClip attackSound;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    void OnAttack(InputValue value)
    {
        if (!value.isPressed) return;

        hitEffect.Play();
        AudioSource.PlayClipAtPoint(attackSound, transform.position);

        GameObject[] allBosses = GameObject.FindGameObjectsWithTag("Boss");

        foreach (GameObject bossObj in allBosses)
        {
            float distance = Vector3.Distance(transform.position, bossObj.transform.position);
            if (distance <= attackRange) bossObj.GetComponent<BossScript2>().TakeDamage(10f, transform.forward);
            // Must comment out above statement because of outdataed TakeDamage() call
        }

    }



}