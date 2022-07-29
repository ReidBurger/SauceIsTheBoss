using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject spawnable;
    [SerializeField]
    private float minSpawnTime = 2f;
    [SerializeField]
    private float maxSpawnTime = 4f;
    [SerializeField]
    private int maxEnemiesOnScreen = 3;
    // enemy min spawn time, max spawn time, speed, accuracy, shoot time min,
    // shoot time max, max enemies on screen
    private float[] enemyData = { 0, 1, 5, 0.8f, 1, 3, 2 };
    private float enemyShootMin;
    private float enemyShootMax;
    private float enemyAccuracy = 0.8f;
    private float enemySpeed;
    public bool bossOut = false;

    private IEnumerator spawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));
            if (transform.childCount < maxEnemiesOnScreen && !bossOut)
            {
                GameObject newSpawn = Instantiate(spawnable, transform.position, Quaternion.identity);
                newSpawn.transform.SetParent(transform);

                Enemy thisEnemy = newSpawn.transform.GetComponent<Enemy>();
                thisEnemy.updateBehavior(enemyShootMin, enemyShootMax, enemyAccuracy, enemySpeed);
            }
        }
    }

    public void updateBehavior(float[] waveEnemyData)
    {
        if (waveEnemyData != null)
        { enemyData = waveEnemyData; }

        minSpawnTime = enemyData[0];
        maxSpawnTime = enemyData[1];
        enemySpeed = enemyData[2];
        enemyAccuracy = enemyData[3];
        enemyShootMin = enemyData[4];
        enemyShootMax = enemyData[5];
        maxEnemiesOnScreen = (int)enemyData[6];
    }

    private void Start()
    {
        StartCoroutine(spawnRoutine());
    }
}
