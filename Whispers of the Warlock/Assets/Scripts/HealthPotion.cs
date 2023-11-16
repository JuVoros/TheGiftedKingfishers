using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    [SerializeField] Collider potion;
    [Range(1, 10)][SerializeField] int HPOnPickup;
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        if (other.CompareTag("Player"))
        {
            gameManager.instance.playerScript.addHealth(HPOnPickup);

        }

        Destroy(gameObject);
    }

}
