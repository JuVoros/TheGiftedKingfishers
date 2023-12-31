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
    [SerializeField] AudioClip damageClip;
    [SerializeField] AudioClip deathClip;
    [SerializeField] AudioClip enemyDeadClip;
    [SerializeField] AudioSource sfxSource;
    [SerializeField] AudioSource musicSource;
    


    [Header("----- Enemy Stats ------")]
    [Range(1, 100)]public int EnemyHP;
    [Range(1, 10)][SerializeField] int playerFaceSpeed;
    [SerializeField] int viewCone;
    [SerializeField] int shootCone;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;
    [SerializeField] int deathAnimation;


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

    [Header("----- Cryo Stats -----")]
    [SerializeField] GameObject ice;


    Vector3 playerDir;
    bool playerInRange;
    bool isShooting;
    bool isDead;
    bool destinationChosen;
    bool isMeleeAttacking = false;
    public int EnemyHPOrig;
    float angleToPlayer;
    public float stoppingDistOrig;
    Vector3 startingPos;
    public int gateIndex;


    void Start()
    {
        EnemyHPOrig = EnemyHP;
        stoppingDistOrig = agent.stoppingDistance;
        startingPos = transform.position;
        if (agent.CompareTag("Enemy"))
        {
            gateIndex += 1;
            enemyHpBar.enabled = false;
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
            StartCoroutine(DeathAnimTimer());
            gameManager.instance.updateGoal(100);
            

            int rando = Random.Range(0, 100);
           

            if (agent.CompareTag("Enemy") && agent.GetComponent<TreeitemDrop>() == null)
            {
                gateIndex -= 1;
                gameManager.instance.updateGoal(500);
                DropItem(gameManager.instance.getWeaponDrops());
                AudioLoop.AudioClipSet(enemyDeadClip, musicSource);
            }
            else if (agent.GetComponent<TreeitemDrop>() != null)
            {
                agent.GetComponent<TreeitemDrop>().DropItem(gameManager.instance.getWeaponDrops());


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
            agent.enabled = false;
            AudioFeedback.Effect(sfxSource, deathClip);

        }
        else
        {
            anim.SetTrigger("Damage");
            StartCoroutine(flashRed());
            agent.SetDestination(gameManager.instance.player.transform.position);
            AudioFeedback.Effect(sfxSource, damageClip);
        }
    }

    void DropItem(List<GameObject> drops)
    {
        if (drops.Count == 0)
        {
            return;
        }


        int drop = Random.Range(0, drops.Count - 1);
        Instantiate(drops[drop], transform.position, transform.rotation);
        drops.RemoveAt(drop);
    }

    void DropPotion(List<GameObject> drops)
    {
        if (drops.Count == 0)
        {
            return;
        }

        int drop = Random.Range(0, 9);
        if (drop > 5)
        {
            Instantiate(drops[0], transform.position, transform.rotation);
        }
        else if (drop < 6 && drop > 2)
        {
            Instantiate(drops[1], transform.position, transform.rotation);
        }
        else
        {
            return;
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
    IEnumerator DeathAnimTimer()
    {

        yield return new WaitForSeconds(deathAnimation);

    }
    public void FreezeEnemy()
    {
        ice.SetActive(true);
    }
    public void UnfreezeEnemy()
    {
        ice.SetActive(false);

    }


    
}
