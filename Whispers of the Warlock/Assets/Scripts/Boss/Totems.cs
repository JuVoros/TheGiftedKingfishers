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
    [SerializeField] int totemHealth = 15;
    
    Vector3 bossDir;
    int totemHealthorig = 15;
    public void Start()
    {

        Boss = GameObject.FindWithTag("Boss");

    }



    public void Update()
    {
        if (gameObject.activeSelf)
            Channel();

    }




    public void takeDamage(int damage)
    {
        totemHealth -= damage;

        if( totemHealth <= 0)
        {
            totem.enabled = false;
            damageColli.enabled = false;
            totemHealth = totemHealthorig;   
        }
        
    }

    void Channel()
    {

        bossDir = Boss.transform.position - headPos.position;
        RaycastHit hit;

        if (Physics.Raycast(headPos.position, bossDir, out hit))
        {
            //if (hit.collider.CompareTag("Player"))
            //{            
            //    StartCoroutine(Shieldbeam());
            //}
        }
    }

    IEnumerator Shieldbeam()
    {

        Instantiate(beam, shootPos.position + transform.forward, transform.rotation);
        yield return new WaitForSeconds(shootRate);

    }




}
