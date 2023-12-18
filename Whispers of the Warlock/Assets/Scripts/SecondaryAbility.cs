using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class SecondaryAbility : MonoBehaviour
{
    [Header("-----Components------")]
    [SerializeField] public AudioSource audi;
    [SerializeField] public GameObject player;
    [SerializeField] TMP_Text textCooldown;
    float cooldownTimer;
    float tempTimer;
    [Header("----- Blink ------")]
    [Range(5, 20)][SerializeField] int teleportDistance;
    [SerializeField] int blinkMana;
    [SerializeField] float blinkCooldownTime; // in Frames
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
    [Range(1, 10)][SerializeField] float shieldCooldownTime;
    [SerializeField] public AudioClip shieldSound;
    [SerializeField] public AudioClip audioShieldRecharge;
    [Header("----- Rock ------")]
    public GameObject rockPrefab;
    public float throwForce = 10f;
    [SerializeField] int rockDamage;
    [SerializeField] int rockSpeed;
    [SerializeField] int rockDestroyTime;
    [SerializeField] AudioClip audioRockRecharge;
    [SerializeField] float rockCooldownTime;
    [SerializeField] int rockMana;
    [SerializeField] List<Transform> Positions = new List<Transform>();
    [Header("----- Shock -----")]
    public GameObject shockPrefab;
    [SerializeField] int shockMana;
    [SerializeField] int shockCooldownTime;

    [Header("----- Cryofreeze ------")]
    [SerializeField] float cryoCooldownTime;
    [SerializeField] int cryoMana;
    [SerializeField] Collider freezeZone;
    [SerializeField] AudioClip audioFreeze;






    //bool
    bool blinkCooldown;
    public bool isShieldActive = false;
    private bool isShieldOnCooldown = false;
    bool rockCooldown;
    bool cryoCooldown;
    bool isCooldown;
    bool shockCooldown;
    public bool cryoTriggerActive;


    //int
    int manaCur;

    //float
   

    //Others
    string staffName;


    private void Start()
    {
        freezeZone.enabled = false;
        textCooldown.gameObject.SetActive(false);
        gameManager.instance.teleportImage.fillAmount = 0.0f;
        gameManager.instance.shieldImage.fillAmount = 0.0f;
        gameManager.instance.earthImage.fillAmount = 0.0f;
        gameManager.instance.lightningImage.fillAmount = 0.0f;
        gameManager.instance.iceImage.fillAmount = 0.0f;

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
                    if (!isShieldOnCooldown && !isCooldown)
                    {
                        ActivateShield();

                    }                   
                    break;

                case "Water":
                    if (!blinkCooldown && !isCooldown)
                    {
                        teleport();
                    }
                    break;

                case "Ice":
                    if (!cryoCooldown && !isCooldown)
                    {
                        Freeze();
                    }
                        break;

                case "Earth":
                    if(!rockCooldown && !isCooldown)
                        SpawnRocks();
                    
                 
                    break;

                case "Lightning":
                    if (!shockCooldown && !isCooldown)
                    {
                        Zap();
                    }
                        break;

            }

        }
        if(isCooldown)
        {

            ApplyCooldown(staffName);

        }


    }




    void ApplyCooldown(string staff)
    {
        

        cooldownTimer -= Time.deltaTime;

        tempTimer = cooldownTimer;

        
        if ( tempTimer < 0.0f )
        {
            textCooldown.gameObject.SetActive(false);
            switch (staff)
            {
                case "Wind":
                    isShieldOnCooldown = false;
                    gameManager.instance.shieldImage.fillAmount = 0.0f;
                    break;
                case "Water":

                    blinkCooldown = false;
                    gameManager.instance.teleportImage.fillAmount = 0.0f;
                    break;
                case "Ice":
                    cryoCooldown = false;
                    gameManager.instance.iceImage.fillAmount = 0.0f;
                    break;
                case "Earth":
                    rockCooldown = false;
                    gameManager.instance.earthImage.fillAmount = 0.0f;
                    break;
                case "Lightning":
                    shockCooldown = false;
                    gameManager.instance.lightningImage.fillAmount = 0.0f;
                    break;
            }
            isCooldown = false;
        }
        else if( tempTimer == 0)
        {
                isShieldOnCooldown = false;
                gameManager.instance.shieldImage.fillAmount = 0.0f;
           
                blinkCooldown = false;
                gameManager.instance.teleportImage.fillAmount = 0.0f;
             
                cryoCooldown = false;
                gameManager.instance.iceImage.fillAmount = 0.0f;
              
                rockCooldown = false;
                gameManager.instance.earthImage.fillAmount = 0.0f;
           
                shockCooldown = false;
                gameManager.instance.lightningImage.fillAmount = 0.0f;


        }
        else
        {
            textCooldown.text = Mathf.RoundToInt(cooldownTimer).ToString();
            switch (staff)
            {
                case "Wind":
                    gameManager.instance.shieldImage.fillAmount = cooldownTimer / shieldCooldownTime;
                    break;
                case "Water":
                    gameManager.instance.playerBlinkFOVdown();
                    gameManager.instance.teleportImage.fillAmount = cooldownTimer / blinkCooldownTime;
                    break;
                case "Ice":
                    gameManager.instance.iceImage.fillAmount = cooldownTimer / cryoCooldownTime;
                    break;
                case "Earth":
                    gameManager.instance.earthImage.fillAmount = cooldownTimer / rockCooldownTime;
                    break;
                case "Lightning":
                    gameManager.instance.lightningImage.fillAmount = cooldownTimer / shockCooldownTime;
                    break;
            }
        }
    }
    //Teleport - Water   

    public void teleport()
    {
        if (manaCur >= blinkMana)
        {
            blinkCooldown = true;
            textCooldown.gameObject.SetActive(true);
            cooldownTimer = blinkCooldownTime;

            gameManager.instance.playerBlinkFOVup();
            gameManager.instance.playerScript.manaCur -= blinkMana;
            gameManager.instance.playerScript.updatePlayerUI();

            Vector3 teleportPosition = player.transform.position + player.transform.forward * teleportDistance;

            if (audioTeleport != null)
            {
                audi.PlayOneShot(audioTeleport);
            }
            gameManager.instance.playerScript.controller.Move(teleportPosition - player.transform.position);

            isCooldown = true;
            blinkCooldown = true;
        }

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
                    Vector3 direction = enemy.transform.position - player.transform.position;
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
    private void ActivateShield()
    {
        if (!isShieldOnCooldown && manaCur >= shieldManaCost)
        {
            audi.PlayOneShot(shieldSound);
            isShieldActive = true;
            shield.SetActive(true);
            gameManager.instance.playerScript.manaCur -= shieldManaCost;
            StartCoroutine(DeactivateShield());
            gameManager.instance.playerScript.updatePlayerUI();
        }
    }
    IEnumerator DeactivateShield()
    {
        yield return new WaitForSeconds(shieldDrainTimer);
        isShieldOnCooldown = true;
        cooldownTimer = shieldCooldownTime;
        textCooldown.gameObject.SetActive(true);
        isCooldown = true;
        audi.PlayOneShot(audioShieldRecharge);
        isShieldActive = false;
        shield.SetActive(false);

    }
    //Yep Rock - Earth
    void SpawnRocks()
    {
        if (manaCur >= rockMana)
        {
            gameManager.instance.earthImage.fillAmount = 0;
            gameManager.instance.playerScript.manaCur -= rockMana;
            gameManager.instance.playerScript.updatePlayerUI();
            for (int i = 0; i < 3; i++) //spawn 3 rocks 
            {
                Instantiate(rockPrefab, Positions[i].position, Quaternion.identity);

            }
            isCooldown = true;
            rockCooldown = true;
            cooldownTimer = rockCooldownTime;
            textCooldown.gameObject.SetActive(true);
        }
    }


    //Elctro - Lightning

    void Zap()
    {
        if (manaCur >= shockMana)
        {
            gameManager.instance.lightningImage.fillAmount = 0;
            gameManager.instance.playerScript.manaCur -= shockMana;
            gameManager.instance.playerScript.updatePlayerUI();


            Quaternion rotation = Quaternion.Euler(0, 90, 0);
            Quaternion postRota = transform.rotation * rotation;




            Instantiate(shockPrefab, Positions[0].transform.position, postRota);



            isCooldown = true;
            shockCooldown = true;
            cooldownTimer = shockCooldownTime;
            textCooldown.gameObject.SetActive(true);
        }
    }

    // Cryofreeze - Ice

    void Freeze()
    {
        if (manaCur >= cryoMana)
        {
            gameManager.instance.iceImage.fillAmount = 0;
            gameManager.instance.playerScript.manaCur -= cryoMana;
            gameManager.instance.playerScript.updatePlayerUI();
            
            StartCoroutine(FreezeTime());
            
            isCooldown = true;
            cryoCooldown = true;
            cooldownTimer = cryoCooldownTime;
            textCooldown.gameObject.SetActive(true);
        }




    }
    IEnumerator FreezeTime()
    {
        freezeZone.enabled = true;
        cryoTriggerActive = true;
        audi.PlayOneShot(audioFreeze);

        yield return new WaitForSeconds(1f);
        
        freezeZone.enabled = false;
        cryoTriggerActive = false;
    }

}
