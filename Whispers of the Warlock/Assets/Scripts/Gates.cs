using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gates : MonoBehaviour
{
    



    void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {

            gameManager.instance.closeGate();

        }
    }
}
