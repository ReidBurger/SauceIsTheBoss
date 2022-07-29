using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private GameObject spawnable;
    [SerializeField]
    private float minSpawnTime = 5f;
    [SerializeField]
    private float maxSpawnTime = 8f;
    [SerializeField]
    private float startSpawnChance = 0.5f;
    [SerializeField]
    private GameObject spawnMaster;
    private SpawnCounter spawnCounter;

    private IEnumerator spawnRoutine()
    {
        GameObject newSpawn = Instantiate(spawnable, transform.position, Quaternion.identity);
        newSpawn.transform.SetParent(transform);
        newSpawn.SetActive(false);
        yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));

        while (!canAddSpawnable())
        {
            // Waiting for a spot to be available
            yield return new WaitForSeconds(1);
        }

        newSpawn.SetActive(true);
    }

    private void Start()
    {
        if (spawnMaster != null)
        {
            spawnCounter = spawnMaster.GetComponent<SpawnCounter>();
        }

        if (Random.Range(0.0f, 1.0f) < startSpawnChance && canAddSpawnable())
        {
            GameObject newSpawn = Instantiate(spawnable, transform.position, Quaternion.identity);
            newSpawn.transform.SetParent(transform);
        }
        else
        {
            StartCoroutine(spawnRoutine());
        }


    }

    private void Update()
    {
        if (transform.childCount == 0)
        {
            if (spawnCounter != null)
            {
                // Has been picked up
                spawnCounter.currentTotal--;
            }
            StartCoroutine(spawnRoutine());
        }
    }

    public void updateBehavior(float[] pastaData)
    {
        minSpawnTime = pastaData[0];
        maxSpawnTime = pastaData[1];
    }

    public bool canAddSpawnable()
    {
        if (spawnCounter != null)
        {
            if (spawnCounter.currentTotal < spawnCounter.maxSpawnCount)
            {
                spawnCounter.currentTotal++;
                return true;
            }
            else return false;
        }
        else return true;
    }
}
