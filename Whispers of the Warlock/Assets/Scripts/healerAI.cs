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
    [SerializeField] GameObject friendlyOne;
    [SerializeField] GameObject friendlyTwo;
    [SerializeField] int viewCone;
    [SerializeField] int shootCone;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;



    [Header("----- Gun Stats -----")]
    [SerializeField] GameObject bullet;
    [Range(0, 10)][SerializeField] float shootRate;

    Vector3 playerDir;
    Vector3 friendlyOneDir;
    Vector3 friendlyTwoDir;
    bool playerInRange;
    bool enemyInRange;
    bool isShooting;
    bool isDead;
    bool destinationChosen;
    int EnemyHPOrig;
    float angleToPlayer;
    float angleToEnemy;
    float stoppingDistOrig;
    Vector3 startingPos;
    Vector3 dropLoca;
    Vector3 placeHolder = new Vector3(3, 2, -10);

    void Start()
    {
        EnemyHPOrig = EnemyHP;
        stoppingDistOrig = agent.stoppingDistance;
        startingPos = transform.position;

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

        if (agent.remainingDistance < 0.1f && !destinationChosen && !isDead)
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
        //if (friendlyOne.GetComponent<NavMeshAgent>().isActiveAndEnabled)
        //{
        //    friendlyOneDir = friendlyOne.transform.position - headPos.position;
        //    angleToEnemy = Vector3.Angle(new Vector3(friendlyOneDir.x, 0, friendlyOneDir.y), transform.forward);
        //}
        //else
        //{
        //    friendlyTwoDir = friendlyTwo.transform.position - headPos.position;
        //    angleToEnemy = Vector3.Angle(new Vector3(friendlyTwoDir.x, 0, friendlyTwoDir.y), transform.forward);
        //}

        Debug.DrawRay(headPos.position, playerDir);

        RaycastHit hit;

        //if (enemyInRange)
        //{
        //    if (Physics.Raycast(headPos.position, Dir, out hit))
        //    {
        //        if (hit.collider.CompareTag("Player") && angleToPlayer <= viewCone)
        //        {
        //            agent.stoppingDistance = stoppingDistOrig;
        //            Debug.Log(agent.remainingDistance);

        //            if (isRangedEnemy && angleToPlayer <= shootCone && !isShooting)
        //            {
        //                StartCoroutine(Shoot());
        //            }

        //            if (agent.remainingDistance < agent.stoppingDistance)
        //            {

        //                faceTarget();
        //            }
        //            agent.SetDestination(gameManager.instance.player.transform.position);
        //            return true;
        //        }
        //    }
            if (!enemyInRange)
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

            Quaternion rot = Quaternion.LookRotation(playerDir);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);
        }

    }
}

    

