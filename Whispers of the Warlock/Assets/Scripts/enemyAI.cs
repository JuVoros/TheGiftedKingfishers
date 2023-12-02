using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class enemyAI : MonoBehaviour, IDamage
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
    [Range(1, 100)]public int EnemyHP;
    [Range(1, 10)][SerializeField] int playerFaceSpeed;
    [SerializeField] int viewCone;
    [SerializeField] int shootCone;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;


    public Image enemyHpBar;

    [Header("----- Gun Stats -----")]
    [SerializeField] GameObject bullet;
    [Range(0, 10)][SerializeField] float shootRate;

    [Header("----- Melee Stats -----")]
    [SerializeField] int meleeCone;
    [SerializeField] float meleeAttackDuration;
    [SerializeField] float meleeAttackRange;
    [SerializeField] int meleeDamage;
    [SerializeField] float meleeAttackDamageTiming; //timing within the melee attack anim when damage should be applied, adjust based on our anim

    Vector3 playerDir;
    bool playerInRange;
    bool isShooting;
    bool isDead;
    bool destinationChosen;
    bool isMeleeAttacking = false;
    public int EnemyHPOrig;
    float angleToPlayer;
    float stoppingDistOrig;
    Vector3 startingPos;
    Vector3 dropLoca;
    Vector3 placeHolder = new Vector3(3, 2, -10);


    void Start()
    {
        EnemyHPOrig = EnemyHP;
        stoppingDistOrig = agent.stoppingDistance;
        startingPos = transform.position;
        if (agent.CompareTag("Enemy"))
        {
            enemyHpBar.enabled = false;
            gameManager.instance.updateGoal(1);
        }
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
        if (isDead && gameObject.activeSelf)
        {
            CleanUp();

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

    bool canSeePlayer()
    {
        playerDir = gameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);

        Debug.DrawRay(headPos.position, playerDir);
        Debug.Log("Ray");

        RaycastHit hit;

        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewCone)
            {
                agent.stoppingDistance = stoppingDistOrig;

                if (isRangedEnemy && angleToPlayer <= shootCone && !isShooting)
                {
                    StartCoroutine(Shoot());
                } 
                else if (!isRangedEnemy && angleToPlayer <= meleeCone && !isMeleeAttacking && agent.remainingDistance <= meleeAttackRange)
                    StartCoroutine(meleeAttack());

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
            if (agent.CompareTag("Enemy"))
                enemyHpBar.enabled = true;

        }

    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            agent.stoppingDistance = 0;
            if (agent.CompareTag("Enemy"))
                enemyHpBar.enabled = false;

        }

    }

    IEnumerator Shoot()
    {
        isShooting = true;
        anim.SetTrigger("Shoot");
        Instantiate(bullet, shootPos.position + transform.forward, transform.rotation);
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    IEnumerator meleeAttack()
    {
        isMeleeAttacking = true;
        int animIndex = Random.Range(0, 4);
        anim.SetFloat("Index", animIndex);
        anim.SetTrigger("Melee");
        
        //Wait for melee attack anim to reach a point where damage should be applied
        yield return new WaitForSeconds(meleeAttackDamageTiming);

        RaycastHit hit;
        if(Physics.Raycast(headPos.position, playerDir, out hit, meleeAttackRange))
        {
            if(hit.collider.CompareTag("Player"))
            {
                hit.collider.GetComponent<IDamage>().takeDamage(meleeDamage);
            }
        }

        //Waits for rest of melee attack anim to finish
        yield return new WaitForSeconds(meleeAttackDuration - meleeAttackDamageTiming);

        isMeleeAttacking = false;
    }

    public void takeDamage(int damage)
    {
        
        EnemyHP -= damage;
        
        if(agent.CompareTag("Enemy"))
            updateHpBar();
  
        if (EnemyHP <= 0)
        {
            isDead = true;
            damageColli.enabled = false;
            anim.SetBool("Death", true);
            agent.enabled = false;
            
            
            int rando = Random.Range(0, 100);

            if (agent.CompareTag("Enemy"))
            {
                gameManager.instance.updateGoal(-1);
                DropItem(gameManager.instance.getWeaponDrops());

            }
            else if(rando == 99 )
            {
               
                    DropItem(gameManager.instance.getWeaponDrops());
            }
            else
            {
                DropPotion(gameManager.instance.getPotionDrops());

                if (rando == 59 || rando == 60)
                {
                    gameManager.instance.ScareJump();
                }

            }
                      

        }
        else
        {
            anim.SetTrigger("Damage");
            StartCoroutine(flashRed());
            agent.SetDestination(gameManager.instance.player.transform.position);
        }
    }

    void DropItem(List<GameObject> drops)
    {
        if (drops.Count == 0)
        {
            return;
        }

        dropLoca = transform.position + placeHolder;

        int drop = Random.Range(0, drops.Count - 1);
        Instantiate(drops[drop], dropLoca, transform.rotation);
        drops.RemoveAt(drop);
    }

    void DropPotion(List<GameObject> drops)
    {
        if (drops.Count == 0)
        {
            return;
        }

        dropLoca = transform.position + placeHolder;

        int drop = Random.Range(0, drops.Count - 1);
        Instantiate(drops[drop], dropLoca, transform.rotation);
        
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

    public void updateHpBar()
    {
        enemyHpBar.fillAmount = (float)EnemyHP / EnemyHPOrig;

    }
    void CleanUp()
    {

        if ((transform.position - gameManager.instance.player.transform.position).magnitude > 90)
        {

            Destroy(gameObject, 5f);
        }
    }
    
}
