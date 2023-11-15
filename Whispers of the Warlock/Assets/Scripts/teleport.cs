using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class teleport : MonoBehaviour
{
    public float teleportDistance = 10f;

    private CharacterController characterController;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q)) 
        {
        Vector3 teleportPosition = transform.position + transform.forward * teleportDistance;
            characterController.Move( teleportPosition - transform.position);
            
        }
            
    }
}
