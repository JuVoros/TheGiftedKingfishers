using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotemBullet : MonoBehaviour
{
    [Header("---- Components ----")]
    [SerializeField] Rigidbody rb;
    [SerializeField] GameObject Boss;


    [Header("---- Stats ----")]
    [SerializeField] int damage;
    [SerializeField] int speed;
    [SerializeField] float destroyTime;
    Vector3 yOffset = new Vector3(0,8,0);


    // Start is called before the first frame update
    void Start()
    {
        Boss = GameObject.FindWithTag("Boss");

        Vector3 dir = (Boss.transform.position + yOffset) - rb.position;
        dir /= Time.deltaTime;
        dir = Vector3.ClampMagnitude(dir, speed);
        rb.velocity = dir;

        Destroy(gameObject, destroyTime);
    }
}
