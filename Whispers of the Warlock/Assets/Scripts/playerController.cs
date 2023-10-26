using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    [Header("-----Components------")]
    [SerializeField] CharacterController controller;

    [Header("------Player Stats------")]
    [Range(1, 10)] [SerializeField] float jumpHeight;
    [Range(1, 10)] [SerializeField] float playerSpeed;
    [Range(-5, -20)] [SerializeField] float gravityValue;
    [Range(1, 10)] [SerializeField] int jumpsMax;

    [Header("------Gun Stats------")]
    [Range(1, 100)][SerializeField] int shootDamage;
    [Range(1, 100)][SerializeField] int shootDist;
    [Range(0, 2)][SerializeField] float shootRate;

    private Vector3 playerVelocity;
    private Vector3 move;
    private bool isGrounded;
    private int jumpedTimes;
    bool isShooting;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Ray tracing
        Debug.DrawRay(Camera.main.transform.position, 
            Camera.main.transform.forward * shootDist, Color.red);


        //checks if player is on the ground
        isGrounded = controller.isGrounded;
        if(isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            jumpedTimes = 0;
        }
        //getting button input
        move = Input.GetAxis("Horizontal") * transform.right +
            Input.GetAxis("Vertical") * transform.forward;
        //moving the player
        controller.Move(move * Time.deltaTime * playerSpeed);
        //check for jumps
        if(Input.GetButtonDown("Jump") && jumpedTimes < jumpsMax) 
        {
            playerVelocity.y = jumpHeight;
            jumpedTimes++;
        }
        //move again
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }
}
