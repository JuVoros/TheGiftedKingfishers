using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gates : MonoBehaviour
{
    [SerializeField] GameObject text;



    void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
                text.SetActive(true);         
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            text.SetActive(false); 
        }
    }
}
