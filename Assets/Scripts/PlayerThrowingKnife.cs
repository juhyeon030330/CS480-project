using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerThrowingKnife : MonoBehaviour
{
    public Transform camera;
    public Transform attackPoint;
    public GameObject knifeObject;

    public int totalKnives;
    public float throwCooldown;

    public KeyCode throwKey = KeyCode.F;
    public float throwForce;
    public float throwUpwardForce;

    bool readyToThrow;

    private void Start()
    {
        readyToThrow = true;
    }

    private void Update() {
        if (Input.GetKeyDown(throwKey) && readyToThrow && totalKnives > 0) {
            Throw();
        }
    }

    private void Throw()
    {
        readyToThrow = false;

        GameObject knife = Instantiate(knifeObject, attackPoint.position, camera.rotation);

        Rigidbody knifeRB = knife.GetComponent<Rigidbody>();

        Vector3 forceDirection = camera.transform.forward;

        RaycastHit hit;

        if (Physics.Raycast(camera.position, camera.forward, out hit, 500f))
        {
            forceDirection = (hit.point - attackPoint.position).normalized;
        }

        Vector3 forceToAdd = forceDirection * throwForce + transform.up * throwUpwardForce;

        knifeRB.AddForce(forceToAdd, ForceMode.Impulse);

        totalKnives--;

        Invoke(nameof(ResetThrow), throwCooldown);
    }

    private void ResetThrow()
    {
        readyToThrow = true;
    }
}
