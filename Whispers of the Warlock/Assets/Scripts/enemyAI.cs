using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class enemyAI : MonoBehaviour, IDamage
{
    [Header("----- Componets ------")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;
    

    [Header("----- Enemy Stats ------")]
    [Range(1, 100)][SerializeField] int EnemyHP;
    [Range(1, 10)][SerializeField] int playerFaceSpeed;
    [SerializeField] int viewCone;
    [SerializeField] int shootCone;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;
    public Image enemyHpBar;

    [Header("----- Gun Stats -----")]
    [SerializeField] GameObject bullet;
    [Range(0, 10)][SerializeField] float shootRate;

    Vector3 playerDir;
    bool playerInRange;
    bool isShooting;
    float angleToPlayer;
    int EnemyHPOrig;
    float stoppingDistOrig;
    bool destinationChosen;
    Vector3 startingPos;

    void Start()
    {
        EnemyHPOrig = EnemyHP;
        stoppingDistOrig = agent.stoppingDistance;
        startingPos = transform.position;
        if (agent.CompareTag("Enemy"))

            gameManager.instance.updateGoal(1);
        
    }

    void Update()
    {

        if (agent.isActiveAndEnabled)
        {
          
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

        if (agent.remainingDistance < 0.05f && !destinationChosen)
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

    bool canSeePlayer()
    {
        playerDir = gameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);

        Debug.DrawRay(headPos.position, playerDir);
        Debug.Log(angleToPlayer);

        RaycastHit hit;

        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewCone)
            {
                agent.stoppingDistance = stoppingDistOrig;


                if (angleToPlayer <= shootCone && !isShooting)
                    StartCoroutine(Shoot());

                if (agent.remainingDistance < agent.stoppingDistance)
                {
                    faceTarget();
                }
                agent.SetDestination(gameManager.instance.player.transform.position);

                return true;
            }
        }
        agent.stoppingDistance = 0;
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

    IEnumerator Shoot()
    {
        isShooting = true;
        
        Instantiate(bullet, shootPos.position + transform.forward, transform.rotation);
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    public void takeDamage(int damage)
    {
        EnemyHP -= damage;
        
        if(agent.CompareTag("Enemy"))
            updateHpBar();
        StartCoroutine(flashRed());



        if (EnemyHP <= 0)
        {
            if (agent.CompareTag("Enemy"))
            {
                gameManager.instance.updateGoal(-1);
                if (gameManager.instance.enemiesRemaining == 2)
                {
                    gameManager.instance.openGate1(); 
                }
                else if (gameManager.instance.enemiesRemaining == 1)
                {
                    gameManager.instance.openGate2();
                }

            }
            Destroy(gameObject);
        }
    }
    IEnumerator flashRed()
    {
       
        model.GetComponent<Renderer>().sharedMaterial.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.GetComponent<Renderer>().sharedMaterial.color = Color.white;


        //model.material.color = Color.red;
        //yield return new WaitForSeconds(0.2f);
        //model.material.color = Color.white;
    }


    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);
    }

    void updateHpBar()
    {
        enemyHpBar.fillAmount = (float)EnemyHP / EnemyHPOrig;

    }
}
