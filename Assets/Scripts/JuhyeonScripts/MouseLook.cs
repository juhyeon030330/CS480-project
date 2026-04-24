using UnityEngine;

public class MouseLook : MonoBehaviour {
    public float mouseSensitivity = 2200f;
    public Transform playerBody;
    float xRotation = 0f;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update() {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        if (mouseX > 0) {Debug.Log(mouseX);}


        xRotation += mouseX;
        // xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // transform.localRotation = Quaternion.Euler(0f, xRotation, 0f);
        playerBody.localRotation = Quaternion.Euler(0f, xRotation, 0f);
    }
}
