using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickup : MonoBehaviour
{

    [SerializeField] gunStats gun;


    void Start()
    {

    }

    void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {

            gameManager.instance.playerScript.getGunStats(gun);
            Destroy(gameObject);


        }


    }


}
