using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] float lingerTime;
    [SerializeField] ParticleSystem explosionEffect;
    // Start is called before the first frame update
    void Start()
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, explosionEffect.transform.rotation);
        }
        Destroy(gameObject, lingerTime);
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }



        IDamage damageable = other.GetComponent<IDamage>();

        if (damageable != null)
        {
            damageable.takeDamage(damage);
        }
    }
}
