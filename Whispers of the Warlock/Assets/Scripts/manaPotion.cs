using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class manaPotion : MonoBehaviour
{
    [Range(1, 10)][SerializeField] int ManaOnPickup;

    public void consumeMana()
    {

        gameManager.instance.playerScript.addMana(ManaOnPickup);
        Debug.Log("AddMana");
    }


}
