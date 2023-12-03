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
    [SerializeField] int totemsToSpawn;
    [SerializeField] GameObject totem1;
    [SerializeField] GameObject totem2;
    [SerializeField] GameObject totem3;
    [SerializeField] GameObject totem4;
    [SerializeField] GameObject meleeSpawn;
    [SerializeField] GameObject rangeSpawn;


    //int
    int animToPlay;
    int spawnCount;

    //bool
    bool isAttacking = false;
    bool isMelee;
    bool isShooting;
    bool isDead;
    public bool meleeRange;
    bool playerInRange;
    bool isShielding = false;
    bool isSpawning;
    bool totemSpawned;

    //Vectors
    Vector3 playerDir;

    //List
    List<int> animList = new List<int>();

    ImpSpawner spawner;
    public int enemyHpOrig;



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
            
        }
        else if (!isShielding && isAttacking && !isDead)
        {
            HandleAttackingState();
            agent.SetDestination(gameManager.instance.player.transform.position);
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

        enemyHp -= damage;
       
        if (enemyHp <= 0)
        {
            damageCollider.enabled = false;
            anim.SetBool("Death", true);
            agent.enabled = false;
            isDead = true;
        }
        else
        {
            if (enemyHp <= (float)enemyHpOrig * 0.75 && enemyHp > (float)enemyHpOrig * 0.72)
            {
                isShielding = true;
                isAttacking = false;
                totemsToSpawn = 1;
                rangeSpawn.GetComponent<ImpSpawner>().startSpawn(4);
                meleeSpawn.GetComponent<ImpSpawner>().startSpawn(5);
            }
            else if (enemyHp <= (float)enemyHpOrig * 0.5 && enemyHp > (float)enemyHpOrig * 0.47)
            {

                isShielding = true;
                isAttacking = false;
                totemsToSpawn = 2;
                rangeSpawn.GetComponent<ImpSpawner>().startSpawn(6);
                meleeSpawn.GetComponent<ImpSpawner>().startSpawn(6);
            }
            else if (enemyHp <= (float)enemyHpOrig * 0.25 && enemyHp > (float)enemyHpOrig * 0.22)
            {
                isShielding = true;
                isAttacking = false;
                totemsToSpawn = 4;
                rangeSpawn.GetComponent<ImpSpawner>().startSpawn(10);
                meleeSpawn.GetComponent<ImpSpawner>().startSpawn(6);
            }
            anim.SetTrigger("Damage");
            StartCoroutine(flashRed());
        }
    }
    void faceTarget()
    {

        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);

    }   
    void Shield()
    {

        damageCollider.enabled = false;
        
        switch(totemsToSpawn)
        {
            case 1:

                if (!totemSpawned)
                {
                    totem1.GetComponent<Totems>().totem.enabled = true;
                    totem1.GetComponent<Totems>().damageColli.enabled = true;
                    totemSpawned = true;
                }
                break;

            case 2:
                if (!totemSpawned)
                {
                    totem1.GetComponent<Totems>().totem.enabled = true;
                    totem1.GetComponent<Totems>().damageColli.enabled = true;
                    totem4.GetComponent<Totems>().totem.enabled = true;
                    totem4.GetComponent<Totems>().damageColli.enabled = true;
                    totemSpawned = true;
                }
                break;

            case 4:
                if (!totemSpawned)
                {
                    totem1.GetComponent<Totems>().totem.enabled = true;
                    totem1.GetComponent<Totems>().damageColli.enabled = true;
                    totem2.GetComponent<Totems>().totem.enabled = true;
                    totem2.GetComponent<Totems>().damageColli.enabled = true;
                    totem3.GetComponent<Totems>().totem.enabled = true;
                    totem3.GetComponent<Totems>().damageColli.enabled = true;
                    totem4.GetComponent<Totems>().totem.enabled = true;
                    totem4.GetComponent<Totems>().damageColli.enabled = true;
                    totemSpawned = true;
                }
                break;
        }

        if(!totem1.GetComponent<Totems>().totem.enabled && !totem2.GetComponent<Totems>().totem.enabled && 
        !totem3.GetComponent<Totems>().totem.enabled && !totem4.GetComponent<Totems>().totem.enabled)
        {

            isShielding = false;
            isAttacking = true;
            totemSpawned = false;
            damageCollider.enabled = true;

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

            //Wait for melee attack anim to reach a point where damage should be applied
            yield return new WaitForSeconds(meleeAttackDamageTiming);

            RaycastHit hit;
            if (Physics.Raycast(headPos.position, playerDir, out hit, meleeAttackRange))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    hit.collider.GetComponent<IDamage>().takeDamage(meleeDamage);
                }
            }

            //Waits for rest of melee attack anim to finish
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
}
