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
    [Range(1,10)] [SerializeField] int HP;
  

    [Header("------Gun Stats------")]
    [Range(1, 100)][SerializeField] int shootDamage;
    [Range(1, 100)][SerializeField] int shootDistance;
    [Range(0, 2)][SerializeField] float shootRate;

    private Vector3 playerVelocity;
    private Vector3 move;
    private bool isGrounded;
    private int jumpedTimes;
    bool isShooting;
    int HPOrig;

    void Start()
    {
        HPOrig = HP;
        spawnPlayer();
    }

    void Update()
    {
        //Ray tracing
        Debug.DrawRay(Camera.main.transform.position, 
            Camera.main.transform.forward * shootDistance, Color.red);

        if (Input.GetButton("Shoot") && !isShooting)
            StartCoroutine(shoot());

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

    IEnumerator shoot()
    {
        isShooting = true;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDistance))
        {
            IDamage damageable = hit.collider.GetComponent<IDamage>();
            if (hit.transform != transform && damageable != null)
            {
                damageable.takeDamage(shootDamage);
            }
        }



        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }
    public void takeDamage(int amount)
    {
        HP -= amount;
        updatePlayerUI();
        StartCoroutine(gameManager.instance.playerFlashDamage());
        if (HP <= 0)
        {
            gameManager.instance.Lose();
        }
    }

    public void spawnPlayer()
    {

        controller.enabled = false;  
        HP = HPOrig;
        updatePlayerUI();
        transform.position = gameManager.instance.playerSpawnPos.transform.position;
        controller.enabled = true;

    }

    public void updatePlayerUI()
    {
        gameManager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
    }
}
