using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SecondaryAbility : MonoBehaviour
{
    [Header("-----Components------")]
    [SerializeField] public AudioSource audi;
    [SerializeField] public GameObject player;


    [Header("----- Blink ------")]
    [Range(5, 20)][SerializeField] int teleportDistance;
    [SerializeField] int blinkMana;
    [SerializeField] int blinkCooldown; // in Frames
    [SerializeField] AudioClip audioTeleport;
    [SerializeField] AudioClip audioTeleportRecharge;

    [Header("----- Shield ------")]
    [SerializeField] GameObject shield;
    [Range(1, 5)][SerializeField] int shieldManaCost;
    [Range(1, 5)][SerializeField] float shieldDrainTimer;
    [Range(5, 10)][SerializeField] float shieldRadius;
    [Range(5, 10)][SerializeField] float bossShieldRadius;
    [Range(1, 10)][SerializeField] float shieldPushForce;
    [Range(1, 10)][SerializeField] float bossShieldPushForce;
    [Range(1, 10)][SerializeField] float shieldCooldown;
    [SerializeField] public AudioClip shieldSound;
    [SerializeField] public AudioClip deactivateShieldSound;

    [Header("----- Rock ------")]
    public GameObject rockPrefab;
    public float throwForce = 10f;
    [SerializeField] int rockDamage;
    [SerializeField] int rockSpeed;
    [SerializeField] int rockDestroyTime;
    [SerializeField] AudioClip audioRockRecharge;
    [SerializeField] int rockCooldown;
    [SerializeField] int rockMana;
    Transform rockOne;
    Transform rockTwo;
    Transform rockThree;



    [SerializeField] List<Transform> Positions = new List<Transform>();


    [Header("----- Cryofreeze ------")]


    [Header("----- Shock ------")]





    //bool
    bool isBlinking;
    public bool isShieldActive = false;
    private bool isHoldingShieldButton = false;
    private bool isShieldOnCooldown = false;
    bool rechargeStarted;
    bool rocksSpawned;
    bool rockRechargeStarted;

    //int
    int blinkTimer;
    int manaCur;
    int manaOrig;
    int staffSelscted;

    //float
    private float shieldDrainTimerOrig;
    private float shieldCooldownTimer = 0f;

    //Others
    string staffName;


    private void Start()
    {
        blinkTimer = blinkCooldown;
        shieldDrainTimerOrig = shieldDrainTimer;
        manaOrig = gameManager.instance.playerScript.manaMax;



    }
    void Update()
    {
        manaCur = gameManager.instance.playerScript.manaCur;
        staffName = gameManager.instance.playerScript.weaponName;
        if (Input.GetButtonDown("Special")) 
        {
            switch (staffName)
            {
                case "Wind":
                    if (!isShieldActive)
                    {
                        Debug.Log("shield");
                        ActivateShield();

                    }
                    else if (isShieldActive)
                    {

                        DeactivateShield();

                    }
                    break;

                case "Water":
                    if (!isBlinking)
                    {
                        Debug.Log("blink");
                        teleport();
                    }
                    break;

                case "Ice":


                    break;

                case "Earth":
                    
                    SpawnRocks();
                    
                 
                    break;

                case "Lightning":


                    break;

            }

        }
        BlinkEnd();
        RockEnd();




    }



    //Teleport - Water   

    public void teleport()
    {
        if (!isBlinking)
        {

            if (manaCur >= blinkMana)
            {
                isBlinking = true;
                gameManager.instance.playerBlinkFOVup();
                gameManager.instance.teleportImage.fillAmount = 0;
                gameManager.instance.playerScript.manaCur -= blinkMana;
                gameManager.instance.playerScript.updatePlayerUI();
                Vector3 teleportPosition = player.transform.position + player.transform.forward * teleportDistance;

                if (audioTeleport != null)
                {
                    audi.PlayOneShot(audioTeleport);
                }
                gameManager.instance.playerScript.controller.Move(teleportPosition - player.transform.position);
            }
        }
    }
    public void BlinkEnd()
    {
        if (isBlinking || !rechargeStarted)
        {
            gameManager.instance.teleportImage.fillAmount += 1 / blinkCooldown * Time.deltaTime;
            gameManager.instance.playerBlinkFOVdown();

            if (gameManager.instance.teleportImage.fillAmount >= 1 - audioTeleportRecharge.length / blinkCooldown && rechargeStarted == false)
            {

                rechargeStarted = true;
                StartCoroutine(teleportCooldown());

            }
        }
    }

    IEnumerator teleportCooldown()
    {
        
        if (audioTeleportRecharge != null)
        {
            audi.PlayOneShot(audioTeleportRecharge);
        }
        yield return new WaitForSeconds(audioTeleportRecharge.length);
        isBlinking = false;
        rechargeStarted = false;
        Debug.Log("recharge");
        gameManager.instance.teleportImage.fillAmount = 1;
    }



    //Shield - Wind
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
    private void DrainManaWhileShielding()
    {
        shieldDrainTimer -= Time.deltaTime;
        int shieldDrainRate = shieldManaCost + Mathf.RoundToInt(1);

        if (shieldDrainTimer <= 0f)
        {

            int remainingMana = Mathf.Clamp(manaCur - shieldDrainRate, 0, manaOrig);
            gameManager.instance.playerScript.updatePlayerUI();

            if (remainingMana == 0)
            {
                DeactivateShield();
                return;
            }

            manaCur = remainingMana;
            shieldDrainTimer = shieldDrainTimerOrig;
        }
    }

    private void ActivateShield()
    {
        if (!isShieldOnCooldown && manaCur >= shieldManaCost)
        {
            audi.PlayOneShot(shieldSound);
            isShieldActive = true;
            shield.SetActive(true);
            manaCur -= shieldManaCost;
            gameManager.instance.playerScript.updatePlayerUI();


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
        audi.PlayOneShot(deactivateShieldSound);
        isShieldActive = false;
        shield.SetActive(false);
    }
    //Yep Rock - Earth
    void SpawnRocks()
    {
        if (manaCur >= rockMana)
        {
            gameManager.instance.earthImage.fillAmount += 1 / blinkCooldown * Time.deltaTime;
            gameManager.instance.playerScript.manaCur -= rockMana;
            gameManager.instance.playerScript.updatePlayerUI();
            for (int i = 0; i < 3; i++) //spawn 3 rocks 
            {
                Instantiate(rockPrefab, Positions[i].position, Quaternion.identity);

            }
            rocksSpawned = true;
        }
    }
    public void RockEnd()
    {
        if (isBlinking || !rechargeStarted)
        {
            
            if (gameManager.instance.earthImage.fillAmount >= 1 - audioRockRecharge.length / rockCooldown && rockRechargeStarted == false)
            {

                rechargeStarted = true;
                StartCoroutine(RockCooldown());

            }
        }
    }
  
    IEnumerator RockCooldown()
    {

        if (audioRockRecharge != null)
        {
            audi.PlayOneShot(audioRockRecharge);
        }
        yield return new WaitForSeconds(audioRockRecharge.length);
        isBlinking = false;
        rockRechargeStarted = false;
        Debug.Log("recharge");
        gameManager.instance.earthImage.fillAmount = 1;
    }


    //Elctro - Lightning



    // Cryofreeze - Ice





}
