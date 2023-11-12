using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{
    [Header("-----Components------")]
    [SerializeField] CharacterController controller;
    [SerializeField] AudioSource audi;

    [Header("------Audio------")]
    [SerializeField] AudioClip[] audioDamage;
    [SerializeField] float audioDamageVol;
    [SerializeField] AudioClip[] audioJump;
    [SerializeField] float audioJumpVol;
    [SerializeField] AudioClip[] audioSteps;
    [SerializeField] float audioStepsVol;

    [Header("------Player Stats------")]
    [Range(1, 10)][SerializeField] float jumpHeight;
    [Range(1, 35)][SerializeField] float playerSpeed;
    [Range(10, 35)][SerializeField] float sprintSpeed;
    [Range(-5, -20)][SerializeField] float gravityValue;
    [Range(1, 10)][SerializeField] int jumpsMax;
    [Range(1, 20)][SerializeField] int HP;

    [Header("------Staff Stats------")]
    [SerializeField] List<GunStats> staffList = new List<GunStats>();
    [SerializeField] GameObject staffModel;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDistance;
    [SerializeField] float shootRate;

    int staffSelected;
    int PlayerHPOrig;
    private int jumpedTimes;

    private float speedOrig;

    private Vector3 playerVelocity;
    private Vector3 move;

    private bool isGrounded;
    bool isShooting;


    void Start()
    {
        PlayerHPOrig = HP;
        speedOrig = playerSpeed;
        spawnPlayer();
    }

    void Update()
    {
        Move();
    }
    public void Move()
    {
        //Ray tracing
        Debug.DrawRay(Camera.main.transform.position,
            Camera.main.transform.forward * shootDistance, Color.red);

        //if (Input.GetButton("Shoot") && !isShooting)
        //    StartCoroutine(shoot());

        //checks if player is on the ground
        isGrounded = controller.isGrounded;
        if (isGrounded && playerVelocity.y < 0)
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
        if (Input.GetButtonDown("Jump") && jumpedTimes < jumpsMax)
        {
            playerVelocity.y = jumpHeight;
            jumpedTimes++;
        }
        if (Input.GetButtonDown("Sprint"))
        {
            playerSpeed = sprintSpeed;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            playerSpeed = speedOrig;
        }
        //move again
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }


    //IEnumerator shoot()
    //{
    //    isShooting = true;

    //    RaycastHit hit;
    //    if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDistance))
    //    {
    //        IDamage damageable = hit.collider.GetComponent<IDamage>();
    //        if (hit.transform != transform && damageable != null)
    //        {
    //            damageable.takeDamage(shootDamage);
    //        }
    //    }



    //    yield return new WaitForSeconds(shootRate);
    //    isShooting = false;
    //}
    public void takeDamage(int amount)
    {
        HP -= amount;
        updatePlayerUI();
        if (amount > 0)
        {
            StartCoroutine(gameManager.instance.playerFlashDamage());
        }
        else if(amount <= 0)
        {
            StartCoroutine(gameManager.instance.playerFlashHeals());
        }
        if (HP <= 0)
        {
            gameManager.instance.Lose();
        }
    }

    public void spawnPlayer()
    {

        controller.enabled = false;
        HP = PlayerHPOrig;
        updatePlayerUI();
        transform.position = gameManager.instance.playerSpawnPos.transform.position;
        controller.enabled = true;

    }

    public void updatePlayerUI()
    {
        gameManager.instance.playerHPBar.fillAmount = (float)HP / PlayerHPOrig;
    }

    public void getGunStats(GunStats gun)
    {
        staffList.Add(gun);

        shootDamage = gun.shootDamage;
        shootDistance = gun.shootDistance;
        shootRate = gun.shootRate;

        staffModel.GetComponent<MeshFilter>().sharedMesh = gun.model.GetComponent<MeshFilter>().sharedMesh;
        staffModel.GetComponent<MeshRenderer>().sharedMaterial = gun.model.GetComponent<MeshRenderer>().sharedMaterial;


        staffSelected = staffList.Count - 1;

    }
}
