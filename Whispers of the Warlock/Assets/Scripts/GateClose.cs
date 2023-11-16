using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GateClose : MonoBehaviour
{
    [SerializeField]Animator anim;



    void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {

            gameManager.instance.closeGate();
            anim.SetTrigger("Close");
        }
    }
}
