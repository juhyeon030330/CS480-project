using UnityEngine;

public class KnifeBehavior : MonoBehaviour
{
    private Rigidbody RB;

    private bool targetHit;

    private void Start()
    {
        RB = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (targetHit)
            return;
        else
            targetHit = true;

        RB.isKinematic = true;

        transform.SetParent(collision.transform);
    }


}
