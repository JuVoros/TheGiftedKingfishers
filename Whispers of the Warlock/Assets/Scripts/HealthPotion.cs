using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : MonoBehaviour
{

    [Range(1, 10)][SerializeField] int HpOnPickup;

    public void consumeHP()
    {

        gameManager.instance.playerScript.addHealth(HpOnPickup);
    }


}
