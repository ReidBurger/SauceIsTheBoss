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

    private IEnumerator spawnRoutine()
    {
        GameObject newSpawn = Instantiate(spawnable, transform.position, Quaternion.identity);
        newSpawn.transform.SetParent(transform);
        newSpawn.SetActive(false);
        yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));
        newSpawn.SetActive(true);
    }

    private void Start()
    {
        //gameManager.
        if (Random.Range(0.0f, 1.0f) < startSpawnChance)
        {
            GameObject newSpawn = Instantiate(spawnable, transform.position, Quaternion.identity);
            newSpawn.transform.SetParent(transform);
        }
    }

    private void Update()
    {
        if (transform.childCount == 0)
        {
            StartCoroutine(spawnRoutine());
        }
    }

    public void updateBehavior(float[] pastaData)
    {
        minSpawnTime = pastaData[0];
        maxSpawnTime = pastaData[1];
    }
}
