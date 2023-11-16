using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bul : MonoBehaviour
{
    [Header("---- Components ----")]
    [SerializeField] Rigidbody rb;
    [SerializeField] GameObject explosion;

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
    public IEnumerator takeDamage()
    {
        yield return new WaitForSeconds(destroyTime);

        if (explosion != null)
            Instantiate(explosion, transform.position, explosion.transform.rotation);
        Destroy(gameObject);
    }

}                                                           