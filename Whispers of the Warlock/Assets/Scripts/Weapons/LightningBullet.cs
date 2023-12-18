using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningBullet : MonoBehaviour
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
           
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 dir = (hit.point - transform.position).normalized;
                rb.velocity = dir * speed;
                Debug.DrawRay(dir, rb.velocity, Color.yellow);

            }
            else
            {
                Vector3 dir = playerCamera.transform.forward;
                rb.velocity = dir * speed;
                Debug.DrawRay(dir, rb.velocity, Color.magenta);

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
        else if (other.CompareTag("Enemy") || other.CompareTag("Foe") || other.CompareTag("Boss") || !other.CompareTag("Totem"))
        {

            other.gameObject.AddComponent<Hit>();

            Hit h = gameObject.GetComponent<Hit>();

            IDamage damagable = other.GetComponent<IDamage>();

            if (damagable != null && h == null )
            {
                damagable.takeDamage(damage);
                DamagePopup.Create(other.transform.position, damage);
            }
        }
    }
}
