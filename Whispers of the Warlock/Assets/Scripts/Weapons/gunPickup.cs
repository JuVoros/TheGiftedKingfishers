using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickup : MonoBehaviour
{

    [SerializeField] gunStats gun;
    public GameObject attackPointPrefab;


    void Start()
    {

    }

    void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {

            gameManager.instance.playerScript.getGunStats(gun, attackPointPrefab);
            Destroy(gameObject);


        }


    }


}
