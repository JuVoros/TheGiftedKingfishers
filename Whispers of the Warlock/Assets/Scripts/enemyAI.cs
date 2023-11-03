using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
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

    [Header("----- Gun Stats -----")]
    [SerializeField] GameObject bullet;
    [Range(0, 10)][SerializeField] float shootRate;

    Vector3 playerDir;
    bool playerInRange;
    bool isShooting;
    Color collideColor = Color.red;
    Color normalColor = Color.white;
    void Start()
    {
        model.GetComponent<Renderer>().sharedMaterial.color = normalColor;
        if (agent.CompareTag("Enemy"))
            gameManager.instance.updateGoal(1);
        
    }

    void Update()
    {
        if (playerInRange)
        {
            playerDir = gameManager.instance.player.transform.position - transform.position;
            
            if(!isShooting)
            {
                StartCoroutine(Shoot());
            }
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                faceTarget();
            }

            agent.SetDestination(gameManager.instance.player.transform.position);

        }
        
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
            agent.SetDestination(agent.transform.position);



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
        StartCoroutine(flashRed());



        if (EnemyHP <= 0)
        {
            if (agent.CompareTag("Enemy"))
            {
                gameManager.instance.updateGoal(-1);
            }
            Destroy(gameObject);
        }
    }
    IEnumerator flashRed()
    {
       
        model.GetComponent<Renderer>().sharedMaterial.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.GetComponent<Renderer>().sharedMaterial.color = normalColor;


        //model.material.color = Color.red;
        //yield return new WaitForSeconds(0.2f);
        //model.material.color = Color.white;
    }


    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);
    }
}
