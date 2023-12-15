using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.Image;

public class playerController : MonoBehaviour, IDamage
{
    [Header("-----Components------")]
    [SerializeField] CharacterController controller;
    [SerializeField] public AudioSource audi;

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
    [SerializeField] public AudioClip heartBeat;
    [SerializeField] float heartbeatVol;
    [SerializeField] public AudioClip shieldSound;
    [SerializeField] float shieldSoundVol;
    [SerializeField] public AudioClip deactivateShieldSound;
    [SerializeField] float deactivateShieldSoundVol;

    [Header("------Player Stats------")]
    [Range(1, 10)][SerializeField] float jumpHeight;
    [Range(1, 35)][SerializeField] float playerSpeed;
    [Range(10, 35)][SerializeField] float sprintSpeed;
    [Range(-5, -20)][SerializeField] float gravityValue;
    [Range(1, 10)][SerializeField] int jumpsMax;
    [Range(1, 120)][SerializeField] public int HP;
    [Range(5, 20)][SerializeField] int teleportDistance;
    [SerializeField] int blinkMana;
    [SerializeField] int blinkCooldown; // in Frames
    int blinkTimer;
    [Range(1, 200)][SerializeField] public int manaMax;
    [Range(1, 5)][SerializeField] int manaPerRegen;
    [SerializeField] float fallYLevel;

    public int PlayerHPOrig;
    private int jumpedTimes;
    public int manaCur;
    [Header("------Low Health Screens------")]
    [SerializeField] GameObject stageOneLow;
    [SerializeField] GameObject stageTwoLow;
    [SerializeField] GameObject stageThreeLow;
    [SerializeField] GameObject stageFourLow;
    [SerializeField] GameObject stageFiveLow;

    //Gun Stats
    [SerializeField] List<gunStats> staffList = new List<gunStats>();
    [SerializeField] GameObject staffModel;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDistance;
    [SerializeField] float shootRate;
    [SerializeField] float rechargeRate;
    [SerializeField] string weaponName;
    [SerializeField] public Light staffLight;
    private GameObject currentStaffModel;
    int staffSelected;

    [Header("------Shield Stats------")]
    [Range(1, 5)][SerializeField] int shieldManaCost;
    //[Range(1, 5)][SerializeField] int shieldDrainRate;
    [Range(1, 5)][SerializeField] float shieldDrainTimer;
    [Range(5, 10)][SerializeField] float shieldRadius;
    [Range(5, 10)][SerializeField] float bossShieldRadius;
    [Range(1, 10)][SerializeField] float shieldPushForce;
    [Range(1, 10)][SerializeField] float bossShieldPushForce;
    [Range(1, 10)][SerializeField] float shieldCooldown;
    [SerializeField] GameObject shield;
    public bool isShieldActive = false;
    private bool isHoldingShieldButton = false;
    private float shieldDrainTimerOrig;
    private playerPrefsManager prefsManager;
    private float speedOrig;
    private float shieldCooldownTimer = 0f;
    private bool isShieldOnCooldown = false;

    private Vector3 playerVelocity;
    private Vector3 move;
    GameObject descendant;

    private bool isGrounded;
    bool isShooting;
    bool isPlayingSteps;
    bool isSprinting;
    bool isRegenMana;
    bool isBlinking;
    bool rechargeStarted;
    bool isLowHealthFlashing = false;
    void Awake()
    {
        PlayerHPOrig = HP;
        manaCur = manaMax;
        speedOrig = playerSpeed;
        blinkTimer = blinkCooldown;
        shieldDrainTimerOrig = shieldDrainTimer;
        prefsManager = GetComponent<playerPrefsManager>();
        spawnPlayer();

    }

    void Update()
    {
        if (transform.position.y < fallYLevel)
        {
            spawnPlayer();
            gameManager.instance.turnOffHealthBar();
        }
        if (!gameManager.instance.isPaused)
        {
            Move();
            teleport();


            if (staffList.Count > 0)
            {
                selectStaff();

                // Reload();
                if (!isRegenMana)
                {
                    StartCoroutine(manaRegen());
                }


                if (GetKeyDown("Shoot") && !isShooting)
                    StartCoroutine(shoot());
            }

            if (PlayerHPOrig * .5 >= HP && !isLowHealthFlashing)
            {
                StartCoroutine(playHeartbeat());

            }
            if ((float)PlayerHPOrig * 0.5 < HP)
            {
                stageOneLow.SetActive(false);
                stageTwoLow.SetActive(false);
                stageThreeLow.SetActive(false);
                stageFourLow.SetActive(false);
                stageFiveLow.SetActive(false);
            }


            if (GetKeyDown("Shield") && !isHoldingShieldButton && manaCur >= shieldManaCost)
            {
                isHoldingShieldButton = true;
                gameManager.instance.ChangeIconAlpha(gameManager.instance.shieldIcon, 0.4f);
                ActivateShield();
            }
            else if (GetKeyUp("Shield") && isHoldingShieldButton)
            {
                isHoldingShieldButton = false;
                gameManager.instance.ChangeIconAlpha(gameManager.instance.shieldIcon, 1.0f);
                DeactivateShield();
            }

            if(isShieldActive)
            {
                DrainManaWhileShielding();
            }

        }
    }

    void FixedUpdate()
    {
        if (isShieldActive)
        {
            UpdateStoppingDistance("Enemy");
            UpdateStoppingDistance("Boss");
            UpdateStoppingDistance("Foe");
        }
    }

    void UpdateStoppingDistance(string tag)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(tag);

        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
            {
                enemyAI enemyAI = enemy.GetComponent<enemyAI>();
                BossScript bossScript = enemy.GetComponent<BossScript>();

                if (enemyAI != null)
                {
                    Vector3 direction = enemy.transform.position - transform.position;
                    direction.y = 0f; // Ignore vertical distance
                    float distanceToEnemy = direction.magnitude;

                    float newStoppingDistance = enemyAI.stoppingDistOrig * shieldRadius;

                    NavMeshAgent enemyAgent = enemy.GetComponent<NavMeshAgent>();
                    enemyAgent.stoppingDistance = newStoppingDistance;
                    if (distanceToEnemy <= shieldRadius && enemyAgent.enabled)
                    {
                        Vector3 pushDirection = direction.normalized;
                        enemyAgent.Move(pushDirection * shieldPushForce * Time.fixedDeltaTime);
                    }
                }

                else if (bossScript != null)
                {
                    // Handle stopping distance and pushing logic for BossScript
                    Vector3 direction = enemy.transform.position - transform.position;
                    direction.y = 0f; // Ignore vertical distance
                    float distanceToEnemy = direction.magnitude;

                    float newStoppingDistance = bossScript.stoppingDistOriginal * shieldRadius;

                    NavMeshAgent enemyAgent = enemy.GetComponent<NavMeshAgent>();
                    enemyAgent.stoppingDistance = newStoppingDistance;

                    if (distanceToEnemy <= shieldRadius)
                    {
                        Vector3 pushDirection = direction.normalized;
                        enemyAgent.Move(pushDirection * bossShieldPushForce * Time.fixedDeltaTime);
                    }
                }
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
        //move = Input.GetAxis("Horizontal") * transform.right +
        //    Input.GetAxis("Vertical") * transform.forward;
        float horizontalInput = prefsManager.GetAxisFromKeybind(playerPrefsManager.GameAction.MoveRight) - prefsManager.GetAxisFromKeybind(playerPrefsManager.GameAction.MoveLeft);
        float verticalInput = prefsManager.GetAxisFromKeybind(playerPrefsManager.GameAction.MoveUp) - prefsManager.GetAxisFromKeybind(playerPrefsManager.GameAction.MoveDown);

        move = horizontalInput * transform.right + verticalInput * transform.forward;


        //moving the player
        controller.Move(move * Time.deltaTime * playerSpeed);

        //check for jumps
        if (GetKeyDown("Jump") && jumpedTimes < jumpsMax)
        {
            audi.PlayOneShot(audioJump[Random.Range(0, audioJump.Length)], audioJumpVol);
            playerVelocity.y = jumpHeight;
            jumpedTimes++;
        }
        if (GetKeyDown("Sprint"))
        {
            isSprinting = true;
            playerSpeed = sprintSpeed;
        }
        else if (GetKeyUp("Sprint"))
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
        if (manaCur > 0 && !isShieldActive)
        {
            isShooting = true;
            manaCur--;

            updatePlayerUI();
            audi.PlayOneShot(staffList[staffSelected].shootSound, staffList[staffSelected].shootSoundVol);

            string weaponName = staffList[staffSelected].weaponName;

           GameObject attackPoint = AttackPointManager.instance.GetAttackPoint(weaponName);

            if (attackPoint != null)
            {
                Vector3 spawnPosition = attackPoint.transform.position;
                Vector3 spawnDirection = Camera.main.transform.forward;

                GameObject bullet = Instantiate(staffList[staffSelected].bulletPrefab, spawnPosition, Quaternion.LookRotation(spawnDirection));
            }
            else
            {
                Debug.LogError("Attack Point Not Found for weapon: " + weaponName);
            }

                yield return new WaitForSeconds(shootRate);
                isShooting = false;
        }
    }
     

public void takeDamage(int amount)
    {
        HP -= amount;
        updatePlayerUI();
        if (PlayerHPOrig * 0.5 >= HP)
            SetLowScreen(HP, PlayerHPOrig);
        if (HP > 0)
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
    public GameObject returnChild(GameObject parent, string descendantName)
    {

        foreach(Transform child in parent.transform)
        {
            if(child.name == descendantName)
            {
                descendant = child.gameObject;
                break;

            }
            else
            {
                returnChild(child.gameObject, descendantName);

            }
        }
            return descendant;

    }

    public void getGunStats(gunStats gun, GameObject attackPointPrefab)
    {
        string descendantName = "Weapon Holder";
        GameObject weaponHolder = returnChild(gameManager.instance.player, descendantName);
        
        Transform weaponTransform = weaponHolder.transform;
        foreach (Transform child in weaponTransform)
        {
            child.gameObject.SetActive(false);
        }
        staffList.Add(gun);

        shootDamage = gun.shootDamage;
        shootDistance = gun.shootDistance;
        shootRate = gun.shootRate;
        rechargeRate = gun.rechargeRate;
        weaponName = gun.weaponName;
        gameManager.instance.weaponNameUpdate();
        audi.PlayOneShot(pickupSound, pickupVol);

        if (weaponTransform != null)
        {
            GameObject staffObject = Instantiate(gun.model, weaponTransform.transform.position, weaponTransform.transform.rotation);
            staffObject.transform.SetParent(transform);

            staffObject.SetActive(false);

            string attackPointName = weaponName + "AttackPoint";

            Transform attackPoint = staffObject.transform.Find(attackPointName);

            if (attackPoint != null)
            {
                AttackPointManager.AttackPoint point = new AttackPointManager.AttackPoint
                {
                    weaponName = weaponName,
                    pointTransform = attackPoint.gameObject
                };
                AttackPointManager.instance.AddAttackPoint(point);
            }
            else
            {
              
                if (attackPointPrefab != null)
                {
                    attackPoint = Instantiate(attackPointPrefab, staffObject.transform).transform;
                    attackPoint.gameObject.name = attackPointName;

                    AttackPointManager.AttackPoint point = new AttackPointManager.AttackPoint
                    {
                        weaponName = weaponName,
                        pointTransform = attackPoint.gameObject
                    };
                    AttackPointManager.instance.AddAttackPoint(point);
                }
                else
                {
                    Debug.LogError("Attack Point Not Found on staffObject! Make sure the name matches");
                }
            }

            staffModel.GetComponent<MeshFilter>().sharedMesh = gun.model.GetComponent<MeshFilter>().sharedMesh;
            staffModel.GetComponent<MeshRenderer>().sharedMaterial = gun.model.GetComponent<MeshRenderer>().sharedMaterial;
            gameManager.instance.weaponNameUpdate();

            staffSelected = staffList.Count - 1;
        }
        else
        {
            Debug.LogError("Weapon Holder not found!");
        }
    }

    IEnumerator manaRegen()
    {
        isRegenMana = true;
        gameManager.instance.updateGoal(5);
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
        StartCoroutine(speedBuff());
            HP += amount;
            audi.PlayOneShot(potionSound, potionVol);

        if (HP >= PlayerHPOrig)
        {
            HP = PlayerHPOrig;
        }
        updatePlayerUI() ;
    }

    public void teleport()
    {
        if (GetKeyDown("Blink") && !isBlinking)
        {
            if (manaCur >= blinkMana)
            {
                isBlinking = true;
                gameManager.instance.playerBlinkFOVup();
                gameManager.instance.updateGoal(25);
                gameManager.instance.teleportIcon.fillAmount = 0;
                manaCur -= blinkMana;
                updatePlayerUI();
                Vector3 teleportPosition = transform.position + transform.forward * teleportDistance;

                if (audioTeleport != null)
                {
                    audi.PlayOneShot(audioTeleport, audioTeleportVol);
                }
                controller.Move(teleportPosition - transform.position);
            }
        }

        if (isBlinking)
        {
            gameManager.instance.teleportIcon.fillAmount += 1 / blinkCooldown * Time.deltaTime;
            gameManager.instance.playerBlinkFOVdown();

            if (gameManager.instance.teleportIcon.fillAmount >= 1 - audioTeleportRecharge.length/blinkCooldown && rechargeStarted == false)
            {

                rechargeStarted = true;
                StartCoroutine(teleportCooldown());
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

    IEnumerator teleportCooldown()
    {

        if (audioTeleportRecharge != null)
        {
            audi.PlayOneShot(audioTeleportRecharge, audioTeleportRechargeVol);
        }
        yield return new WaitForSeconds(audioTeleportRecharge.length);
        isBlinking = false;
        rechargeStarted = false;
        gameManager.instance.teleportIcon.fillAmount = 1;
    }
    public void SetLowScreen(int Hp, int original)
    {
        if ((float)original * 0.5 >= Hp && (float)original * 0.4 < Hp)
            stageOneLow.SetActive(true);
        else if ((float)original * 0.4 >= Hp && (float)original * 0.3 < Hp)
        {
            stageOneLow.SetActive(false);
            stageTwoLow.SetActive(true);
        }
        else if ((float)original * 0.3 >= Hp && (float)original * 0.2 < Hp)
        {
            stageTwoLow.SetActive(false);
            stageThreeLow.SetActive(true);
        }
        else if ((float)original * 0.2 >= Hp && (float)original * 0.1 < Hp)
        {
            stageThreeLow.SetActive(false);
            stageFourLow.SetActive(true);
        }
        else if ((float)original * 0.1 >= Hp)
        {
            stageFourLow.SetActive(false);
            stageFiveLow.SetActive(true);
        }
        else if((float)original * 0.5 < Hp)
        {
            stageOneLow.SetActive(false);
            stageTwoLow.SetActive(false);
            stageThreeLow.SetActive(false);
            stageFourLow.SetActive(false);
            stageFiveLow.SetActive(false);
        }

    }
    IEnumerator playHeartbeat() 
    {
        isLowHealthFlashing = true;
        audi.PlayOneShot(heartBeat, heartbeatVol);
        yield return new WaitForSeconds(14f * HP/PlayerHPOrig);
        isLowHealthFlashing = false;

    }

    private void DrainManaWhileShielding()
    {
        shieldDrainTimer -= Time.deltaTime;
        Debug.Log("Shield Drain Timer: " + shieldDrainTimer);
        int shieldDrainRate = shieldManaCost + Mathf.RoundToInt(rechargeRate);

        if (shieldDrainTimer <= 0f)
        {
            
            int remainingMana = Mathf.Clamp(manaCur - shieldDrainRate, 0, manaMax);
            updatePlayerUI();
            Debug.Log("Mana Drained: " + remainingMana);

            if(remainingMana == 0)
            {
                DeactivateShield();
                return;
            }

            manaCur = remainingMana;
            shieldDrainTimer = shieldDrainTimerOrig;
            Debug.Log("Shield Drain Timer Reset: " + shieldDrainTimer);
        }
    }

    private void ActivateShield()
    {
        if(!isShieldOnCooldown && manaCur >= shieldManaCost)
        {
            audi.PlayOneShot(shieldSound, shieldSoundVol);
            isShieldActive = true;
            shield.SetActive(true);
            manaCur -= shieldManaCost;
            updatePlayerUI();

            shieldDrainTimer = shieldDrainTimerOrig;
            startShieldCooldown();
        }
    }

    private void startShieldCooldown()
    {
        isShieldOnCooldown = true;
        StartCoroutine(ShieldCooldown());
    }

    private IEnumerator ShieldCooldown()
    {
        float cooldownDuration = shieldCooldown;

        yield return new WaitForSeconds(cooldownDuration);

        isShieldOnCooldown = false;
    }
    private void DeactivateShield()
    {
        audi.PlayOneShot(deactivateShieldSound, deactivateShieldSoundVol);
        isShieldActive = false;
        shield.SetActive(false);
    }

   bool GetKeyDown(string action)
{
    return Input.GetKey(prefsManager.GetKeybind(playerPrefsManager.GameActionFromString(action)));
}

bool GetKeyUp(string action)
{
    return Input.GetKeyUp(prefsManager.GetKeybind(playerPrefsManager.GameActionFromString(action)));
}

    IEnumerator speedBuff()
    {
        int speedMod = Mathf.FloorToInt(playerSpeed * .25f);

        playerSpeed += speedMod;
        sprintSpeed += speedMod;
        yield return new WaitForSeconds(2f);
        playerSpeed -= speedMod;
        sprintSpeed -= speedMod;
    }
}
