using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    [SerializeField] Collider potion;
    [Range(-10,-1)][SerializeField] int HPOnPickup;
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        IDamage damageable = other.GetComponent<IDamage>();

        if (other.CompareTag("Player")&&damageable!=null)
        {
            damageable.takeDamage(HPOnPickup);
            
        }
        
        Destroy(gameObject);
    }

}
