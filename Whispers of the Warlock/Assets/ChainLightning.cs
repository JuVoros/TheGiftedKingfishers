using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainLightning : MonoBehaviour
{

    private Collider coll;
    public LayerMask enemy;
    public int damage;

    public GameObject ChainlightingEffect;
    public GameObject Hit;
    public int amountToChain;
    private GameObject startobject;
    public GameObject endObject;

    private Animator anim;

    public ParticleSystem parti;

    private int singleSpawns;



    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Shock");
        if(amountToChain == 0)
            Destroy(gameObject);
        coll = GetComponent<Collider>();
        anim = GetComponent<Animator>();

        startobject = gameObject;
        
        singleSpawns = 1;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.GetComponent<Hit>());
        if (other.CompareTag("Player") || other.isTrigger)
            return;
        else if (other.GetComponent<Hit>() == null)
        {
            Debug.Log(singleSpawns);

            if (singleSpawns != 0)
            {

                Debug.Log("If state");
                endObject = other.gameObject;
                amountToChain -= 1;
                Instantiate(ChainlightingEffect, other.gameObject.transform.position, Quaternion.identity);
                Instantiate(Hit, other.gameObject.transform);

                IDamage damagable = other.GetComponent<IDamage>();

                if (damagable != null)
                {
                    damagable.takeDamage(damage);
                    DamagePopup.Create(other.transform.position, damage);
                }

                anim.StopPlayback();

                coll.enabled = false;
                singleSpawns--;

                parti.Play();

                var emitParams = new ParticleSystem.EmitParams();
                emitParams.position = startobject.transform.position;
                parti.Emit(emitParams, 1); 

                emitParams.position = endObject.transform.position;
                parti.Emit(emitParams, 1);



                Destroy(gameObject, 1f);
            }

        }



    }

}
