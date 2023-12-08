using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Totems : MonoBehaviour, IDamage
{   
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;
    [SerializeField] public Renderer totem;
    [SerializeField] public Collider damageColli;
    [SerializeField] GameObject beam;
    [SerializeField] GameObject Boss;
    [SerializeField] float shootRate;
    [SerializeField] int totemHealth;
    
    Vector3 bossDir;
    public int totemHealthorig;
    bool isDead;
    public void Start()
    {
        Boss = GameObject.FindWithTag("Boss");
        totemHealthorig = Boss.GetComponent<BossScript>().totemHealth;
        totemHealth = totemHealthorig;
    }



    public void Update()
    {
        if(!isDead)
            Channel();

    }




    public void takeDamage(int damage)
    {
        totemHealth -= damage;

        if( totemHealth <= 0)
        {

            Boss.GetComponent<BossScript>().isAttacking = true;
            Boss.GetComponent<BossScript>().isShielding = false;

            isDead = true;
            Destroy(gameObject);
        }
        
    }

    void Channel()
    {

        bossDir = Boss.transform.position - headPos.position;
        StartCoroutine(Shieldbeam());
    }

    IEnumerator Shieldbeam()
    {
        Instantiate(beam, shootPos.position + (-transform.forward), transform.rotation);
        yield return new WaitForSeconds(shootRate);

    }




}
