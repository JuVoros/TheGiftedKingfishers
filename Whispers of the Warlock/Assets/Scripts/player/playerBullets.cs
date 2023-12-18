using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerBullets : MonoBehaviour
{
    [Header("---- Components ----")]
    [SerializeField] Rigidbody rb;

    [Header("---- Stats ----")]
    [SerializeField] int damage;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;

    void Start()
    {
       
        Camera playerCamera = gameManager.instance.player.GetComponentInChildren<Camera>();

       
        if (playerCamera != null)
        {
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit))
            {
                Vector3 dir = (hit.point - transform.position).normalized;
                rb.velocity = dir * speed;
            }

            Destroy(gameObject, destroyTime);
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other);

        if (other.isTrigger || other.CompareTag("Player") || other.gameObject  == gameObject)
            return;

        if (other.CompareTag("Enemy") || other.CompareTag("Boss") || other.CompareTag("Foe") || other.CompareTag("Totem"))
        {
            // Handle logic for damageable objects (enemies, bosses, foes, triggers)
            IDamage damagable = other.GetComponent<IDamage>();

            if (damagable != null)
            {
                damagable.takeDamage(damage);
                DamagePopup.Create(other.transform.position, damage);
                Destroy(gameObject);
            }
        }
    }

}

