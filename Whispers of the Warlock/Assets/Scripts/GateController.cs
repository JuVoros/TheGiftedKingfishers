using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GateController : MonoBehaviour
{
    [SerializeField] GameObject miniBoss;
    [SerializeField] Animator anim;
    [SerializeField] GameObject message;
    [SerializeField] Collider box;

    NavMeshAgent agent;
    private void Start()
    {
        if (miniBoss != null) 
        {
            agent = miniBoss.GetComponent<NavMeshAgent>();
        }
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!agent.enabled || agent == null)
            {
                anim.SetTrigger("Open");
                box.enabled = false;
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
