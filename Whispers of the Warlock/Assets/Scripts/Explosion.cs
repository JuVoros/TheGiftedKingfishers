using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] int destroyTime;
    [SerializeField] ParticleSystem exploEffect;
    void Start()
    {
        if (exploEffect != null)
            Instantiate(exploEffect, transform.position, exploEffect.transform.rotation);
        
        Destroy(gameObject, destroyTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        IDamage damageable = other.GetComponent<IDamage>();
        StartCoroutine(damagePlayer());

        
        IEnumerator damagePlayer() 
        {
            int interval = 100;

            for (int i = 0; i < interval; i++)
            {
                if(interval % 10 == 0)
                    damageable.takeDamage(damage);
                Debug.Log(i);
                Debug.Log(damagePlayer());
                interval -= 1;
            }


            yield return new WaitForSeconds(destroyTime);

        }

    }
}
