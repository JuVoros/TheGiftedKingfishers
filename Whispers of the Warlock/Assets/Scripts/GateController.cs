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
    private void Update()
    {
        if (miniBoss == null || !agent.enabled )
        {
            OpenGate();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && agent.enabled)
        {
           
                message.SetActive(true);
            
        }
        
    }
    void OpenGate()
    {

        anim.SetTrigger("Open");
        box.enabled = false;
    }



    void OnTriggerExit(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            message.SetActive(false);
        }
    }
}
