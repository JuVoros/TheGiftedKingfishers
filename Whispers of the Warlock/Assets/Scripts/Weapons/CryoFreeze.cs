using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CryoFreeze : MonoBehaviour
{
    NavMeshAgent nav;
    [SerializeField] int freezeTime;

    bool isTriggerActive;
    


    void Update()
    {
        isTriggerActive = gameManager.instance.player.GetComponent<SecondaryAbility>().cryoTriggerActive;


    }



    void OnTriggerStay(Collider other)
    {
        if (isTriggerActive)
        {
            if (other.CompareTag("Enemy") || other.CompareTag("Foe") || other.CompareTag("Boss"))
            {
                nav = other.GetComponent<NavMeshAgent>();

                if (other.CompareTag("Enemy") || other.CompareTag("Foe"))
                {
                    other.GetComponent<enemyAI>().FreezeEnemy();
                }
                else if (other.CompareTag("Boss"))
                {
                    other.GetComponent<BossScript>().FreezeEnemy();

                }
                StartCoroutine(FrozeTime());
            }
        }
    }
    IEnumerator FrozeTime()
    {
        nav.isStopped = true;

        yield return new WaitForSeconds(freezeTime);
        if (nav.CompareTag("Enemy") || nav.CompareTag("Foe"))
        {
            nav.GetComponent<enemyAI>().UnfreezeEnemy();
        }
        else if (nav.CompareTag("Boss"))
        {
            nav.GetComponent<BossScript>().UnfreezeEnemy();
        }



        nav.isStopped = false;


    }
}
