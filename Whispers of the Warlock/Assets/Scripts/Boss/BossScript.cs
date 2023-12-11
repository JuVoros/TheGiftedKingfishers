using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossScript : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] GameObject myself;
    [SerializeField] Renderer model;
    [SerializeField] Animator anim;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] GameObject bullet;
    [SerializeField] Collider damageColli;
    [SerializeField] Collider KneelColli;
    [SerializeField] public GameObject undamageableScreen;

    [Header("----- Stats -----")]
    [SerializeField] public int enemyHp;
    [Range(1, 10)][SerializeField] int playerFaceSpeed;
    [SerializeField] Transform headPos;


    [Header("----- Melee Stats -----")]
    [SerializeField] float meleeAttackRange;
    [SerializeField] public int meleeDamage;
    [SerializeField] float meleeAttackDamageTiming;

    [Header("----- Spawner -----")]
    [SerializeField] GameObject meleeSpawn;
    [SerializeField] GameObject rangeSpawn;
    [SerializeField] GameObject objectToSpawn;


    //int
    int animToPlay;
    public int totemHealth;
    int totemSpawnCount;
    public int enemyHpOrig;
    
    //float
    public float stoppingDistOriginal;

    //bool
    public bool isAttacking = false;
    bool isMelee;
    bool isDead;
    public bool meleeRange;
    bool playerInRange;
    public bool isShielding = false;
    bool totemSpawned;
    bool negateDamage;
    bool playAnim1;

    //Vectors
    Vector3 playerDir;
    Vector3 totemOffset = new Vector3(15,0,0);
    Vector3 totemDrop;


    



    public void Start()
    {
        enemyHpOrig = enemyHp;
        
        stoppingDistOriginal = agent.GetComponent<NavMeshAgent>().stoppingDistance;
        
    }
    public void Update()
    {

        anim.SetFloat("Speed", agent.velocity.normalized.magnitude);
        playerDir = gameManager.instance.player.transform.position - headPos.position;

        if (isShielding && !isAttacking)
        {
            
            HandleShieldingState();
            if (totemSpawnCount == 0)
                StartCoroutine(Spawn());


            model.GetComponent<Renderer>().sharedMaterial.color = Color.blue;





        }
        else if (!isShielding && isAttacking && !isDead)
        {
            HandleAttackingState();
            agent.SetDestination(gameManager.instance.player.transform.position);
            model.GetComponent<Renderer>().sharedMaterial.color = Color.white;

        }


        if (playerInRange && isAttacking && !isDead)
        {

            faceTarget();

        }

        


    }
    void HandleShieldingState()
    {
        agent.SetDestination(myself.transform.position);

        Shield();
    }
    void HandleAttackingState()
    {
        anim.SetBool("Kneel", false);
        damageColli.enabled = true;
        KneelColli.enabled = false;
        totemSpawned = false;
        negateDamage = false;
        totemSpawnCount = 0;


        if (!isMelee && gameManager.instance.player.transform.position.magnitude - transform.position.magnitude <= meleeAttackRange)
        {

            isMelee = true;
            StartCoroutine(MeleeAttack());
            

        }
    }
    public void takeDamage(int damage)
    {
        if(!negateDamage)
        {
            enemyHp -= damage;

            if (enemyHp <= 0)
            {

                anim.SetBool("Death", true);
                agent.enabled = false;
                isDead = true;
                gameManager.instance.updateGoal(2500);
                StartCoroutine(gameManager.instance.Winner());
            }
            else
            {
                if (enemyHp <= (float)enemyHpOrig * 0.75 && enemyHp > (float)enemyHpOrig * 0.73)
                {
                    isShielding = true;
                    isAttacking = false;
                    rangeSpawn.GetComponent<ImpSpawner>().startSpawn(3);
                    meleeSpawn.GetComponent<ImpSpawner>().startSpawn(4);
                    gameManager.instance.updateGoal(250);
                }
                else if (enemyHp <= (float)enemyHpOrig * 0.5 && enemyHp > (float)enemyHpOrig * 0.48)
                {

                    isShielding = true;
                    isAttacking = false;
                    rangeSpawn.GetComponent<ImpSpawner>().startSpawn(5);
                    meleeSpawn.GetComponent<ImpSpawner>().startSpawn(5);
                    gameManager.instance.updateGoal(500);
                }
                else if (enemyHp <= (float)enemyHpOrig * 0.25 && enemyHp > (float)enemyHpOrig * 0.23)
                {
                    isShielding = true;
                    isAttacking = false;
                    rangeSpawn.GetComponent<ImpSpawner>().startSpawn(6);
                    meleeSpawn.GetComponent<ImpSpawner>().startSpawn(6);
                    gameManager.instance.updateGoal(750);
                }
                anim.SetTrigger("Damage");
                StartCoroutine(flashRed());
            }

        }
        else if (negateDamage)
            StartCoroutine(DamagenegateScreen());
        
    }
    void faceTarget()
    {

        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);

    }   
    void Shield()
    {

        anim.SetBool("Kneel", true);
        damageColli.enabled = false;
        KneelColli.enabled = true;
        negateDamage = true;
        if (!totemSpawned)
        {
            totemSpawned = true;
            totemHealth += 5;
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            agent.stoppingDistance = stoppingDistOriginal;
            playerInRange = true;
            isAttacking = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            agent.stoppingDistance = 0;

        }

    }
    IEnumerator MeleeAttack()
    {
        if (!playAnim1)
        {
            animToPlay = 1;
            playAnim1 = true;
        }
        else
        {
            animToPlay = 2;
            playAnim1 = false;
        }

        anim.SetFloat("Index", animToPlay);
        anim.SetTrigger("Melee");

        yield return new WaitForSeconds(meleeAttackDamageTiming);
    
        isMelee = false;

    }
    IEnumerator flashRed()
    {
        model.GetComponent<Renderer>().sharedMaterial.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.GetComponent<Renderer>().sharedMaterial.color = Color.white;
    }
    public IEnumerator Spawn()
    {
        totemDrop = transform.position + totemOffset;

        Instantiate(objectToSpawn,totemDrop,transform.rotation);
        yield return new WaitForSeconds(0);
        totemSpawnCount = 1;
    }
    IEnumerator DamagenegateScreen()
    {

        undamageableScreen.SetActive(true);

        yield return new WaitForSeconds(0.8f);

        undamageableScreen.SetActive(false);
    }

    void InflictDamage()
    {

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit, meleeAttackRange))
        {
            if (hit.collider.CompareTag("Player"))
            {
                hit.collider.GetComponent<IDamage>().takeDamage(meleeDamage);
            }
        }
        


    }
}
