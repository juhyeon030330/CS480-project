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

    //Knife UI Objects
    public GameObject knife1;
    public GameObject knife2;
    public GameObject knife3;

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

        GameObject knife = Instantiate(knifeObject, attackPoint.position, (camera.rotation) * Quaternion.Euler(90, 0, 0));

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

        // Remove knife from UI
        if (totalKnives <= 0)
        {
            knife1.SetActive(false);
        } else if (totalKnives == 1)
        {
            knife2.SetActive(false);
        } else
        {
            knife3.SetActive(false);
        }

        Invoke(nameof(ResetThrow), throwCooldown);
    }

    private void ResetThrow()
    {
        readyToThrow = true;
    }

    public void AddKnife()
    {
        // Add knife to UI
        if (totalKnives <= 1)
        {
            knife1.SetActive(true);
        }
        else if (totalKnives == 2)
        {
            knife2.SetActive(true);
        }
        else
        {
            knife3.SetActive(true);
        }
    }
}
