using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [Header("----- Componets ------")]
    [SerializeField] Rigidbody rb;

    [Header("----- Stats ------")]
    [SerializeField] GameObject explosion;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;
    [SerializeField] int velUp;
    void Start()
    {
        Vector3 dir = gameManager.instance.player.transform.position - rb.position;
        dir /= Time.deltaTime;
        dir = Vector3.ClampMagnitude(dir, speed);
        rb.velocity = dir;
        StartCoroutine(explo());


        IEnumerator explo()
        {
           
            yield return new WaitForSeconds(destroyTime);

            if (explosion != null)
                Instantiate(explosion, transform.position, explosion.transform.rotation);

            Destroy(gameObject);
        }
    }
}
