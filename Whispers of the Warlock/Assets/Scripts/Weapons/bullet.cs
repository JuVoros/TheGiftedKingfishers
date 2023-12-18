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

        SecondaryAbility playerController = gameManager.instance.player.GetComponent<SecondaryAbility>();

        if (playerController != null && playerController.isShieldActive)
        {
            Destroy(gameObject);
        }
        else
        {

            IDamage damagable = other.GetComponent<IDamage>();

            if (damagable != null && other.CompareTag("Player"))
            {
                damagable.takeDamage(damage);
            }
            Destroy(gameObject);
        }

    }

}                                                           