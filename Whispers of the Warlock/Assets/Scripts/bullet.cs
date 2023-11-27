using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bul : MonoBehaviour
{
    [Header("---- Components ----")]
    [SerializeField] Rigidbody rb;

    [Header("---- Stats ----")]
    [SerializeField] int damage;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;



    // Start is called before the first frame update
    void Start()
    {
        Vector3 dir = gameManager.instance.player.transform.position - rb.position;
        dir /= Time.deltaTime;
        dir = Vector3.ClampMagnitude(dir, speed);
        rb.velocity = dir; 

        Destroy(gameObject, destroyTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        Shield shield = other.GetComponent<Shield>();

        if (shield != null && shield.IsShieldActive())
        {
            rb.velocity = -rb.velocity;
        }
        else
        {

        IDamage damagable = other.GetComponent<IDamage>();

        if (damagable != null)
        {
            damagable.takeDamage(damage);
        }
        Destroy(gameObject);
    }

        }

}                                                           