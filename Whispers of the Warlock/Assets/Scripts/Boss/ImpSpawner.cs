using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpSpawner : MonoBehaviour
{
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] public int numToSpawn;
    [SerializeField] float spawnDelay;
    [SerializeField] Transform[] spawmPos;
    [SerializeField] GameObject boss;
    int spawnCount;
    bool isSpawning = false;
    bool startSpawning;
    int enemyHp;
    int enemyHpOrig;

    void Start()
    {
        enemyHp = boss.GetComponent<BossScript>().enemyHp;
        enemyHpOrig = boss.GetComponent<BossScript>().enemyHpOrig;

    }
    void Update()
    {
        if(startSpawning && !isSpawning && spawnCount < numToSpawn)
        {
            StartCoroutine(Spawn());

        }
    }

    public void startSpawn(int amount)
    {
        Debug.Log("spawn");
        spawnCount = 0;
        numToSpawn = amount;
        startSpawning = true;

    }



    public IEnumerator Spawn()
    {
        isSpawning = true;

        Instantiate(objectToSpawn, spawmPos[spawnCount].position, spawmPos[spawnCount].rotation);
        yield return new WaitForSeconds(spawnDelay);
        spawnCount++;
        isSpawning = false;

    }
}
