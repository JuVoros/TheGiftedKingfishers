using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage
{
    [Header("----- Componets ------")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;

    [Header("----- Enemy Stats ------")]
    [Range(1, 10)][SerializeField] int HP;
    [Range(1, 10)][SerializeField] int playerFaceSpeed;
    
    Vector3 playerDir;
    void Start()
    {
        gameManager.instance.updateGoal(1);
    }

    void Update()
    {
        if (gameManager.instance.player.transform.position.x <= 20)
        {
            agent.SetDestination(gameManager.instance.player.transform.position);
            
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                faceTarget();      
            }

            playerDir = gameManager.instance.player.transform.position - transform.position;

        }
    }

    public void takeDamage(int damage)
    {
        HP -= damage;
        StartCoroutine(flashRed());

        if (HP <= 0)
        {
            Destroy(gameObject);
            gameManager.instance.updateGoal(-1);
        }
    }
    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = Color.white;
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);
    }
}
