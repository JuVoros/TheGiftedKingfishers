using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{ 
    [SerializeField] gunStats gun;
    [SerializeField] GameObject button;
    public GameObject attackPointPrefab;


    public Item item;
    bool playerInTrigger;
   


    void PickupStaff()
    {
        
        InventoryManager.Instance.Add(item);
        gameManager.instance.playerScript.getGunStats(gun, attackPointPrefab);
        Destroy(gameObject);


    }
    void PickupPotion()
    {
        AddPotions();

        Destroy(gameObject);

    }

    private void Update()
    {
        if (playerInTrigger && Input.GetButtonDown("Interact") && gameObject.CompareTag("Hp"))
        {

            PickupPotion();

        }
        else if (playerInTrigger && Input.GetButtonDown("Interact") && gameObject.CompareTag("Mana"))
        {

            PickupPotion();

        }
        else if (playerInTrigger && Input.GetButtonDown("Interact"))
        {

            PickupStaff();

        }
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
    public void AddPotions()
    {

        if (gameObject.CompareTag("Mana"))
        {
            gameManager.instance.manaPots += 1;

        }
        if (gameObject.CompareTag("Hp"))
        {
            gameManager.instance.hpPots += 1;

        }
    }
}


