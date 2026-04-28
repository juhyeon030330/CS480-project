using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class DestroyOnFinish : MonoBehaviour
{
    private ParticleSystem ps;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        // If the particle system is no longer active (all particles died), destroy this object.
        if (!ps.IsAlive(true))
        {
            Destroy(gameObject);
        }
    }
}