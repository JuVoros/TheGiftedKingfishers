using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GateController : MonoBehaviour
{
    [SerializeField] GameObject miniBoss;
    [SerializeField] Animator anim;
    [SerializeField] GameObject message;

    NavMeshAgent agent;
    private void Start()
    {
        agent = miniBoss.GetComponent<NavMeshAgent>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!agent.enabled)
            {
                anim.SetTrigger("Open");
            }
            else
            {
                message.SetActive(true);
            }
        }
        
    }
    
    void OnTriggerExit(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            message.SetActive(false);
        }
    }
}
