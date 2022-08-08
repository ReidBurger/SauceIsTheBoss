using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    [SerializeField]
    private GameObject boss;
    [SerializeField]
    private GameObject[] enemySpawners;
    private float[] bossData;
    public float sfx_volume;

    private void Start()
    {
        sfx_volume = MainManager.Instance.sfx_vol / 10f;
    }

    public void updateBehavior(float[] waveBossData)
    {
        bossData = waveBossData;
    }

    public void spawnBoss()
    {
        List<Vector2> locations = new List<Vector2>();
        foreach (GameObject spawner in enemySpawners)
        {
            if (spawner.activeSelf)
            {
                locations.Add(spawner.transform.position);
            }
        }

        // Pick random active location
        Vector2 bossSpawnLocation = locations[Random.Range(0, locations.Count - 1)];

        GameObject _newBoss = Instantiate(boss, bossSpawnLocation, Quaternion.identity);
        Boss newBoss = _newBoss.GetComponent<Boss>();
        newBoss.sfx_volume = sfx_volume;

        newBoss.updateBehavior(bossData);
    }
}
