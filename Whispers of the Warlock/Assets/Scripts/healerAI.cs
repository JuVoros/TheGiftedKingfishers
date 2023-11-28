using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class healerAI : MonoBehaviour
{



    [Header("----- Enemy Type ------")]
    [SerializeField] bool isRangedEnemy;

    [Header("----- Componets ------")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;
    [SerializeField] Renderer model;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;
    [SerializeField] Collider damageColli;


    [Header("----- Enemy Stats ------")]
    [Range(1, 100)][SerializeField] int EnemyHP;
    [Range(1, 10)][SerializeField] int playerFaceSpeed;
    
    [SerializeField] int viewCone;
    [SerializeField] int shootCone;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;



    [Header("----- Gun Stats -----")]
    [SerializeField] GameObject bullet;
    [Range(0, 10)][SerializeField] float shootRate;

    [Header("----- Healing Stats -----")]
    [Range(0, 10)][SerializeField] float healRate;
    [Range(1, 100)][SerializeField] int healAmount;
    [SerializeField] GameObject friendly;

    Vector3 playerDir;
    Vector3 friendlyDir;
    bool playerInRange;
    bool enemyInRange;
    bool isShooting;
    bool isHealing;
    int friendlyHpOrig;
    int friendlyHp;
    bool isDead;
    bool destinationChosen;
    int EnemyHPOrig;
    float angleToPlayer;
    float angleToEnemy;
    float stoppingDistOrig;
    Vector3 startingPos;
   

    void Start()
    {
        EnemyHPOrig = EnemyHP;
        stoppingDistOrig = agent.stoppingDistance;
        startingPos = transform.position;
        friendly = GameObject.FindWithTag("Boss");
        friendlyHp = friendly.GetComponent<enemyAI>().EnemyHP;
        friendlyHpOrig = friendly.GetComponent<enemyAI>().EnemyHPOrig;
    }

    void Update()
    {

        if (agent.isActiveAndEnabled)
        {
            anim.SetFloat("Speed", agent.velocity.normalized.magnitude);

            if (playerInRange && !canSeePlayer())
            {
                StartCoroutine(roam());


            }
            else if (!playerInRange)
            {
                StartCoroutine(roam());
            }



        }

    }

    IEnumerator roam()
    {

        if (agent.remainingDistance < 0.4f && !destinationChosen && !isDead)
        {
            destinationChosen = true;
            agent.stoppingDistance = 0;
            yield return new WaitForSeconds(roamPauseTime);

            Vector3 randomPos = Random.insideUnitSphere * roamDist;
            randomPos += startingPos;

            NavMeshHit hit;
            NavMesh.SamplePosition(randomPos, out hit, roamDist, 1);
            agent.SetDestination(hit.position);


            destinationChosen = false;

        }

    }

    public bool canSeePlayer()
    {
        playerDir = gameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);
        
        
        friendlyDir = friendly.transform.position - headPos.position;
        angleToEnemy = Vector3.Angle(new Vector3(friendlyDir.x, 0, friendlyDir.y), transform.forward);
        Debug.DrawRay(headPos.position, playerDir);

        RaycastHit hit;

        if (friendly.GetComponent<NavMeshAgent>().isActiveAndEnabled)
        {
            if (Physics.Raycast(headPos.position, friendlyDir, out hit))
            {
                if (hit.collider.CompareTag("Boss") && angleToEnemy <= viewCone)
                {
                    agent.stoppingDistance = stoppingDistOrig;

                    if (isRangedEnemy && angleToEnemy <= shootCone && !isHealing && friendlyHp < friendlyHpOrig)
                    {
                        StartCoroutine(Heal());
                        Debug.Log(Heal());
                    }

                    if (agent.remainingDistance < agent.stoppingDistance)
                    {

                        faceTarget();
                    }
                    agent.SetDestination(friendly.transform.position);
                    return true;
                }
            }
        }
        else if(!friendly.GetComponent<NavMeshAgent>().isActiveAndEnabled)
        {

            if (Physics.Raycast(headPos.position, playerDir, out hit))
            {
                if (hit.collider.CompareTag("Player") && angleToPlayer <= viewCone)
                {
                    agent.stoppingDistance = stoppingDistOrig;
                    Debug.Log(agent.remainingDistance);

                    if (isRangedEnemy && angleToPlayer <= shootCone && !isShooting)
                    {
                        StartCoroutine(Shoot());
                    }

                    if (agent.remainingDistance < agent.stoppingDistance)
                    {

                        faceTarget();
                    }
                    agent.SetDestination(gameManager.instance.player.transform.position);

                    return true;
                }
            }
            agent.stoppingDistance = 0;
        }
        return false;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
           
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
    IEnumerator Heal()
    {
        isHealing = true;
        
        anim.SetTrigger("Heal");
        
        if ((friendlyHp += healAmount) > friendlyHpOrig)
        {
            friendly.GetComponent<enemyAI>().EnemyHP = friendly.GetComponent<enemyAI>().EnemyHPOrig;

        }
        else
        {
            friendly.GetComponent<enemyAI>().EnemyHP += healAmount;
        }

        //friendly.GetComponent<enemyAI>().updateHpBar();
        yield return new WaitForSeconds(healRate);
        isHealing = false;
    }

    IEnumerator Shoot()
    {
        isShooting = true;
        anim.SetTrigger("Shoot");
        Instantiate(bullet, shootPos.position + transform.forward, transform.rotation);
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    public void takeDamage(int damage)
    {

        EnemyHP -= damage;



        if (EnemyHP <= 0)
        {
            isDead = true;
            damageColli.enabled = false;
            anim.SetBool("Death", true);
            agent.enabled = false;


        }
        else
        {
            anim.SetTrigger("Damage");
            StartCoroutine(flashRed());
            agent.SetDestination(gameManager.instance.player.transform.position);
        }
    }
    IEnumerator flashRed()
    {

        model.GetComponent<Renderer>().sharedMaterial.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.GetComponent<Renderer>().sharedMaterial.color = Color.white;
    }

    void faceTarget()
    {

        Quaternion rot = Quaternion.LookRotation(friendlyDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);
    }

}



