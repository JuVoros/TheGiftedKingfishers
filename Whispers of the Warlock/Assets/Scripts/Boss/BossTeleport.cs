using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTeleport : MonoBehaviour
{
    [SerializeField] GameObject button;
    [SerializeField] GameObject BossRespawnPoint;
    bool playerInTrigger;


    private void Update()
    {
        if (playerInTrigger && Input.GetButtonDown("Interact"))
        {

            Relocate();            

        }
    }
    void Relocate()
    {
        gameManager.instance.playerSpawnPos = BossRespawnPoint;
        gameManager.instance.turnOnHealthBar();

        gameManager.instance.playerScript.spawnPlayer();



        playerInTrigger = false;
        gameManager.instance.playerSpawnPos = GameObject.FindWithTag("Respawn");
    }
    void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            button.SetActive(true);
            playerInTrigger = true;

        }


    }
    void OnTriggerExit(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            button.SetActive(false);

            playerInTrigger = false;

        }
    }
}
