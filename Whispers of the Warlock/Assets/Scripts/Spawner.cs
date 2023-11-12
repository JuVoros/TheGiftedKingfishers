using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] int numToSpawn;
    [SerializeField] float spawnDelay;
    [SerializeField] Transform[] spawmPos;

    int spawnCount;
    bool isSpawning;
    bool startSpawning;

    void Start()
    {


    }
    void Update()
    {
        if (startSpawning && !isSpawning && spawnCount < numToSpawn)
        {
            StartCoroutine(Spawn());

        }


    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            startSpawning = true;

        }

    }

    IEnumerator Spawn()
    {
        isSpawning = true;
        
        Instantiate(objectToSpawn, spawmPos[spawnCount].position, spawmPos[spawnCount].rotation);
        yield return new WaitForSeconds(spawnDelay);
        spawnCount++;
            

        
        

        
        isSpawning = false;

    }
}