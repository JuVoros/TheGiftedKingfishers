using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class manaPotion : MonoBehaviour
{
    [SerializeField] Collider potion;
    [Range(1, 10)][SerializeField] int ManaOnPickup;
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        if (other.CompareTag("Player"))
        {
            gameManager.instance.playerScript.addMana(ManaOnPickup);

        }

        Destroy(gameObject);
    }

}
