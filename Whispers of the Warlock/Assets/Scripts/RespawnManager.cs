using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{

    [SerializeField] GameObject respawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            gameManager.instance.playerSpawnPos.transform.position = respawnPoint.transform.position;
        }
    }
}