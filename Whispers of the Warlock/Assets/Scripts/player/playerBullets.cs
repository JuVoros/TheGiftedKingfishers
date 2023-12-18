using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerBullets : MonoBehaviour
{
    [Header("---- Components ----")]
    [SerializeField] Rigidbody rb;
    [SerializeField] GameObject Object;
    [SerializeField] GameObject ChainLightningEffect;

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
                Vector3 hitNormal = hit.normal.normalized;
               Vector3 incident = ray.direction.normalized;
                Vector3 reflected = Vector3.Reflect(incident, hitNormal);
                //Vector3 dir = (hit.point - transform.position).normalized;
                //Vector3 dir = Vector3.Reflect(ray.direction, hitNormal);
                rb.velocity = reflected.normalized * speed;
                rb.rotation = Quaternion.identity;
            }
            else
            {
                Vector3 dir = playerCamera.transform.forward;
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
        if (other.isTrigger || other.CompareTag("Player"))
            return;

        HandleCollision(other.gameObject);

    }

    private void HandleCollision(GameObject collidedObject)
    {
        if (collidedObject.CompareTag("Enemy") || collidedObject.CompareTag("Boss") || collidedObject.CompareTag("Foe") || collidedObject.CompareTag("Totem"))
        {
            // Handle logic for damageable objects (enemies, bosses, foes, triggers)
            IDamage damagable = collidedObject.GetComponent<IDamage>();

            if (damagable != null)
            {
                damagable.takeDamage(damage);
                DamagePopup.Create(collidedObject.transform.position, damage);
            }
        }
        else
        {
            // Handle non-damagable collider logic here
            Destroy(gameObject);
        }
    }
}

