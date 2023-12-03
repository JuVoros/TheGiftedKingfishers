using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] int destroyTime;
    [SerializeField] ParticleSystem exploEffect;
    [SerializeField] float damageCooldown;
    float lastDamageTick;


    void Start()
    {
            if (exploEffect != null)
                Instantiate(exploEffect, transform.position, exploEffect.transform.rotation);

            Destroy(gameObject, destroyTime);   
    }



    void OnTriggerStay(Collider other)
    {
        if (Time.time - lastDamageTick < damageCooldown || other.isTrigger)
            return;

        IDamage damageable = other.GetComponent<IDamage>();

        if (other.CompareTag("Player"))
        {
            damageable.takeDamage(damage);

            lastDamageTick = Time.time;

        }




    }














    //void Start()
    //{
    //    if (exploEffect != null)
    //        Instantiate(exploEffect, transform.position, exploEffect.transform.rotation);

    //    Destroy(gameObject, destroyTime);
    //}

    //void OnTriggerEnter(Collider other)
    //{
    //    if (other.isTrigger)
    //        return;

    //    IDamage damageable = other.GetComponent<IDamage>();
    //    StartCoroutine(damagePlayer());


    //    IEnumerator damagePlayer() 
    //    {
    //        int interval = 100;

    //        for (int i = 0; i < interval; i++)
    //        {
    //            if(interval % 10 == 0)
    //                damageable.takeDamage(damage);
    //            Debug.Log(i);
    //            Debug.Log(damagePlayer());
    //            interval -= 1;
    //        }


    //        yield return new WaitForSeconds(destroyTime);

    //    }

    //}
}
