using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{
    [Header("-----Components------")]
    [SerializeField] CharacterController controller;
    [SerializeField] AudioSource audi;
    [SerializeField] GameObject shield;

    [Header("------Audio------")]
    [SerializeField] AudioClip[] audioDamage;
    [SerializeField] float audioDamageVol;
    [SerializeField] AudioClip[] audioJump;
    [SerializeField] float audioJumpVol;
    [SerializeField] AudioClip[] audioSteps;
    [SerializeField] float audioStepsVol;
    [SerializeField] AudioClip audioTeleport;
    [SerializeField] float audioTeleportVol;
    [SerializeField] AudioClip audioTeleportRecharge;
    [SerializeField] float audioTeleportRechargeVol;
    [SerializeField]AudioClip potionSound;
    [SerializeField] float potionVol;
    [SerializeField] AudioClip pickupSound;
    [SerializeField] float pickupVol;

    [Header("------Player Stats------")]
    [Range(1, 10)][SerializeField] float jumpHeight;
    [Range(1, 35)][SerializeField] float playerSpeed;
    [Range(10, 35)][SerializeField] float sprintSpeed;
    [Range(-5, -20)][SerializeField] float gravityValue;
    [Range(1, 10)][SerializeField] int jumpsMax;
    [Range(1, 120)][SerializeField] int HP;
    [Range(5, 20)][SerializeField] int teleportDistance;
    [SerializeField] int blinkMana;
    [SerializeField] int blinkDelay;
    [Range(1, 20)][SerializeField] int manaMax;
    [Range(1, 5)][SerializeField] int manaPerRegen;
    [SerializeField] float fallYLevel;

    int PlayerHPOrig;
    private int jumpedTimes;
    public int manaCur;

    //Gun Stats
    [SerializeField] List<gunStats> staffList = new List<gunStats>();
    [SerializeField] GameObject staffModel;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDistance;
    [SerializeField] float shootRate;
    [SerializeField] float rechargeRate;
    [SerializeField] string weaponName;
    [SerializeField] public Light staffLight;
    int staffSelected;

   
    private float speedOrig;

    private Vector3 playerVelocity;
    private Vector3 move;

    private bool isGrounded;
    bool isShooting;
    bool isPlayingSteps;
    bool isSprinting;
    bool isRegenMana;
    bool isBlinking;



    void Start()
    {
        PlayerHPOrig = HP;
        manaCur = manaMax;
        speedOrig = playerSpeed;
        spawnPlayer();

      
    }

    void Update()
    {
        if (transform.position.y < fallYLevel)
        {
            spawnPlayer();
        }
        if (!gameManager.instance.isPaused)
        {
            Move();

            if (Input.GetButtonDown("Blink")&& !isBlinking)
            {
                StartCoroutine(teleport());
            }

            if (staffList.Count > 0)
            {
                selectStaff();

                // Reload();
                if (!isRegenMana)
                {
                    StartCoroutine(manaRegen());
                }


                if (Input.GetButton("Shoot") && !isShooting)
                    StartCoroutine(shoot());
            }

        }
    }
    public void Move()
    {
        //Ray tracing
        Debug.DrawRay(Camera.main.transform.position,
            Camera.main.transform.forward * shootDistance, Color.red);

        isGrounded = controller.isGrounded;

        if (isGrounded && move.normalized.magnitude > 0.3f && !isPlayingSteps)
            StartCoroutine(playSteps());

        //checks if player is on the ground
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
            audi.PlayOneShot(audioJump[Random.Range(0, audioJump.Length)], audioJumpVol);
            playerVelocity.y = jumpHeight;
            jumpedTimes++;
        }
        if (Input.GetButtonDown("Sprint"))
        {
            isSprinting = true;
            playerSpeed = sprintSpeed;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            isSprinting = false;
            playerSpeed = speedOrig;
        }
        //move again
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    IEnumerator playSteps()
    {
        isPlayingSteps = true;
        audi.PlayOneShot(audioSteps[Random.Range(0, audioSteps.Length)], audioStepsVol);
        if (!isSprinting)
            yield return new WaitForSeconds(0.5f);
        else
            yield return new WaitForSeconds(0.3f);
        isPlayingSteps = false;
    }


    IEnumerator shoot()
    {
        if (manaCur > 0)
        {
            isShooting = true;
            manaCur--;

            updatePlayerUI();
            audi.PlayOneShot(staffList[staffSelected].shootSound, staffList[staffSelected].shootSoundVol);

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDistance))
            {
                Instantiate(staffList[staffSelected].hitEffect, hit.point, staffList[staffSelected].hitEffect.transform.rotation);
                IDamage damageable = hit.collider.GetComponent<IDamage>();


                if (hit.transform != transform && damageable != null)
                {
                    damageable.takeDamage(shootDamage);
                }
            }
            yield return new WaitForSeconds(shootRate);
            isShooting = false;
        }
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        updatePlayerUI();

        if (amount > 0)
        {
            StartCoroutine(gameManager.instance.playerFlashDamage());
            audi.PlayOneShot(audioDamage[Random.Range(0, audioDamage.Length)], audioDamageVol);
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
        gameManager.instance.playerManaBar.fillAmount = (float)manaCur / manaMax;
    }

    void selectStaff()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && staffSelected < staffList.Count - 1)
        {
            staffSelected++;
            changeStaff();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0 && staffSelected == staffList.Count-1)
        {
            staffSelected = 0;
            changeStaff();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && staffSelected > 0)
        {
            staffSelected--;
            changeStaff();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && staffSelected == 0)
        {
            staffSelected  = staffList.Count-1;
            changeStaff();
        }
    }

    void changeStaff()
    {
        weaponName = staffList[staffSelected].weaponName;
        shootDamage = staffList[staffSelected].shootDamage;
        shootDistance = staffList[staffSelected].shootDistance;
        shootRate = staffList[staffSelected].shootRate;
        rechargeRate = staffList[staffSelected].rechargeRate;
        staffModel.GetComponent<MeshFilter>().sharedMesh = staffList[staffSelected].model.GetComponent<MeshFilter>().sharedMesh;
        staffModel.GetComponent<MeshRenderer>().sharedMaterial = staffList[staffSelected].model.GetComponent<MeshRenderer>().sharedMaterial;
        gameManager.instance.weaponNameUpdate();

        isShooting = false;
    }

    public void getGunStats(gunStats gun)
    {
        staffList.Add(gun);
       
        shootDamage = gun.shootDamage;
        shootDistance = gun.shootDistance;
        shootRate = gun.shootRate;
        rechargeRate = gun.rechargeRate;
        weaponName = gun.weaponName;

        audi.PlayOneShot(pickupSound, pickupVol);

        staffModel.GetComponent<MeshFilter>().sharedMesh = gun.model.GetComponent<MeshFilter>().sharedMesh;
        staffModel.GetComponent<MeshRenderer>().sharedMaterial = gun.model.GetComponent<MeshRenderer>().sharedMaterial;
        gameManager.instance.weaponNameUpdate();

        staffSelected = staffList.Count - 1;

    }

    IEnumerator manaRegen()
    {
        isRegenMana = true;

        yield return new WaitForSeconds(rechargeRate);
        if (manaCur >= manaMax)
        {
            manaCur = manaMax;
        }
        else
        {
            manaCur += manaPerRegen;
        }
        updatePlayerUI();
        isRegenMana = false;
    }

    public void addMana(int amount)
    {

            manaCur += amount;
            audi.PlayOneShot(potionSound, potionVol);

        
        if (manaCur >= manaMax)
        {
            manaCur = manaMax;
        }
        updatePlayerUI() ;
    }
    public void addHealth(int amount)
    {

            StartCoroutine(gameManager.instance.playerFlashHeals());
            HP += amount;
            audi.PlayOneShot(potionSound, potionVol);
        

        if(HP >= PlayerHPOrig)
        {
            HP = PlayerHPOrig;
        }
        updatePlayerUI() ;
    }

    public IEnumerator teleport()
    {
        if(manaCur >= blinkMana)
        {
            isBlinking = true;
            manaCur -= blinkMana;
            updatePlayerUI();
        Vector3 teleportPosition = transform.position + transform.forward * teleportDistance;
        if (audioTeleport != null ) 
        {
            audi.PlayOneShot(audioTeleport, audioTeleportVol);
        }
        controller.Move(teleportPosition - transform.position);
            yield return new WaitForSeconds(blinkDelay);
            isBlinking = false;
            if (audioTeleport != null)
            {
                audi.PlayOneShot(audioTeleportRecharge, audioTeleportRechargeVol);
            }
        }
    }
    public IEnumerator jumpScare(GameObject screen, AudioClip clip, float volume)
    {
        screen.SetActive(true);
        audi.PlayOneShot(clip, volume);
        yield return new WaitForSeconds(1f);
        screen.SetActive(false);
    }

    public string getWeaponName()
    {
        return weaponName;
    }
}
