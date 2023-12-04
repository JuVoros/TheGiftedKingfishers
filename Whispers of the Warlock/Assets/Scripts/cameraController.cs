using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{


    [SerializeField] int lockVertMin;
    [SerializeField] int lockVertMax;

    [SerializeField] bool invertY;
    public float sensitivity;
    float xRot;
    
    void Start()
    {
        //Cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

    }

    void Update()
    {
        //input
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity;
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity;

        if (invertY)
        { xRot += mouseY; }
        else
        { xRot -= mouseY; }

        //Clamp the xRot
        xRot = Mathf.Clamp(xRot, lockVertMin, lockVertMax);

        //rotate the camera on the x-axis
        transform.localRotation = Quaternion.Euler(xRot, 0, 0);

        //rotate the camera on the y-axis
        transform.parent.Rotate(Vector3.up * mouseX);
    }

    public void UpdateSensitivity(float newSensitivity)
    {
        sensitivity = newSensitivity;
        Debug.Log($"Sensitivity updated to: {newSensitivity}");
    }
}
