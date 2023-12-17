using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class PhaseHandler : MonoBehaviour
{

    [SerializeField] GameObject boss;
    [Header("----- Phase 1 Stats -----")]
    [SerializeField] int phase1DMG;
    [SerializeField] float normalSpeed;
    [Header("----- Phase 2 Stats -----")]
    [SerializeField] int phase2DMG;
    float speedUp1;


    [Header("----- Phase 3 Stats -----")]
    [SerializeField] int phase3DMG;
    float speedUp2;


    [Header("----- Phase 4 Stats -----")]
    [SerializeField] int phase4DMG;
    float speedUp3;


    float healthOrig;
    private void Start()
    {
        healthOrig = boss.GetComponent<BossScript>().enemyHpOrig;
        normalSpeed = boss.GetComponent<NavMeshAgent>().speed;
        speedUp1 = boss.GetComponent<NavMeshAgent>().speed * (float)1.05;
        speedUp2 = boss.GetComponent<NavMeshAgent>().speed * (float)1.1; 
        speedUp3 = boss.GetComponent<NavMeshAgent>().speed * (float)1.15; 

    }



    void Update()
    {
        int currhealth = boss.GetComponent<BossScript>().enemyHp;
        
        float phase2Trigger = (float)(healthOrig * 0.75);
        float phase3Trigger = (float)(healthOrig * 0.5);

        if (currhealth > phase2Trigger)
        {
            phase1();
        }
        else if( currhealth <= phase2Trigger && currhealth > phase3Trigger)
        {
            phase2();
        }
        else if (currhealth <= phase2Trigger && currhealth > phase3Trigger)
        {
            phase3();
        }
        else
        {
            phase4();

        }               
    }

    void phase1()
    {
        boss.GetComponent<BossScript>().meleeDamage = phase1DMG;
        boss.GetComponent<NavMeshAgent>().speed = normalSpeed;
    }
    void phase2()
    {
        boss.GetComponent<BossScript>().meleeDamage = phase2DMG;
        boss.GetComponent<NavMeshAgent>().speed = speedUp1;
    }
    void phase3()
    {
        boss.GetComponent<BossScript>().meleeDamage = phase3DMG;
        boss.GetComponent<NavMeshAgent>().speed = speedUp2;

    }
    void phase4()
    {
        boss.GetComponent<BossScript>().meleeDamage = phase4DMG;
        boss.GetComponent<NavMeshAgent>().speed = speedUp3;
    }



}
