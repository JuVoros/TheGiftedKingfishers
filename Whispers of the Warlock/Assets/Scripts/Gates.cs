using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gates : MonoBehaviour
{
    [SerializeField] Animator gate;
    [SerializeField] Collider gateClose;

    bool gateOpen;

    public void openGate()
    {
        if (!gateOpen)
        {
            gateOpen = true;
            gate.SetBool("Open", true);

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           
            gate.SetBool("Open", false);

        }
    }
}
