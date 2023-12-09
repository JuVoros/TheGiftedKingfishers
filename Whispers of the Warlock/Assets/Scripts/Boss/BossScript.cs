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
    [SerializeField] Collider damageCollider;
    [SerializeField] public GameObject undamageableScreen;

    [Header("----- Stats -----")]
    [SerializeField] public int enemyHp;
    [Range(1, 10)][SerializeField] int playerFaceSpeed;
    [SerializeField] float shootRate;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;


    [Header("----- Melee Stats -----")]
    [SerializeField] float meleeAttackChaseRange;
    [SerializeField] float meleeAttackDuration;
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

    //bool
    public bool isAttacking = false;
    bool isMelee;
    bool isShooting;
    bool isDead;
    public bool meleeRange;
    bool playerInRange;
    public bool isShielding = false;
    bool totemSpawned;
    bool negateDamage;

    //Vectors
    Vector3 playerDir;
    Vector3 totemOffset = new Vector3(15,0,0);
    Vector3 totemDrop;

    //List
    List<int> animList = new List<int>();

    



    public void Start()
    {
        enemyHpOrig = enemyHp;
        animList.AddRange(new List<int>() {1,2,3,4});
        
    }
    public void Update()
    {
        playerDir = gameManager.instance.player.transform.position - headPos.position;

        if (!isAttacking && !isShielding)
            HandleIdleState();
        else if (isShielding && !isAttacking)
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
    void HandleIdleState()
    {

        anim.SetBool("Idle", true);


    }
    void HandleShieldingState()
    {
        anim.SetBool("Idle", true);
        agent.SetDestination(myself.transform.position);

        Shield();
    }
    void HandleAttackingState()
    {
        anim.SetBool("Idle", false);
        damageCollider.enabled = true;
        totemSpawnCount = 0;
        totemSpawned = false;
        //damageCollider.enabled = true;
        negateDamage = false;

        if ((gameManager.instance.player.transform.position - transform.position).magnitude <= meleeAttackChaseRange && !isMelee & !isShooting)
        {

            isMelee = true;
            StartCoroutine(MeleeAttack());
            

        }
        else if(!isMelee && !isShooting)
        {
            isShooting = true;
            StartCoroutine(Shooting());

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
                    rangeSpawn.GetComponent<ImpSpawner>().startSpawn(4);
                    meleeSpawn.GetComponent<ImpSpawner>().startSpawn(5);
                    gameManager.instance.updateGoal(250);
                }
                else if (enemyHp <= (float)enemyHpOrig * 0.5 && enemyHp > (float)enemyHpOrig * 0.48)
                {

                    isShielding = true;
                    isAttacking = false;
                    rangeSpawn.GetComponent<ImpSpawner>().startSpawn(6);
                    meleeSpawn.GetComponent<ImpSpawner>().startSpawn(6);
                    gameManager.instance.updateGoal(500);
                }
                else if (enemyHp <= (float)enemyHpOrig * 0.25 && enemyHp > (float)enemyHpOrig * 0.23)
                {
                    isShielding = true;
                    isAttacking = false;
                    rangeSpawn.GetComponent<ImpSpawner>().startSpawn(10);
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

        //damageCollider.enabled = false;
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
    IEnumerator Shooting()
    {
        anim.SetTrigger("Shoot");
        Instantiate(bullet, shootPos.position + transform.forward, transform.rotation);
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    IEnumerator MeleeAttack()
    {

        for (int i = 0; i < animList.Count; i++)
        {
            anim.SetFloat("Index", animList[i]);
            anim.SetTrigger("Melee");

            yield return new WaitForSeconds(meleeAttackDamageTiming);

            RaycastHit hit;
            if (Physics.Raycast(headPos.position, playerDir, out hit, meleeAttackRange))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    hit.collider.GetComponent<IDamage>().takeDamage(meleeDamage);
                }
            }
            yield return new WaitForSeconds(meleeAttackDuration - meleeAttackDamageTiming);
        }


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


}
