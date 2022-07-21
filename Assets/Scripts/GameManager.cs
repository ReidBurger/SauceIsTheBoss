using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int kills = 0;
    private int currentWave = 0;

    // array index corresponds to wave number for the following

    // kills to spawn boss
    private float[] waveKillsData = { 15, 20, 25, 30 };

    // pasta min spawn time, max spawn time
    private float[][] wavePastaData =
    {
        new float[2] { 1.2f, 2.2f },
        new float[2] { 0.8f, 1.8f },
        new float[2] { 0.5f, 1.5f },
        new float[2] { 0.4f, 1.4f },
        new float[2] { 0.33f, 1.33f }

    };
    // shield min spawn time, max spawn time, active time
    private float[][] waveShieldData =
    {
        new float[3] { 18, 20, 9.5f },
        new float[3] { 28, 30, 8 },
        new float[3] { 30, 32, 7.5f },
        new float[3] { 34, 36, 7 },
        new float[3] { 38, 40, 6.5f }
    };
    // enemy min spawn time, max spawn time, speed, accuracy, shoot time min,
    // shoot time max, max enemies on screen per spawner
    private float[][] waveEnemyData =
    {
        new float[7] { 3, 5, 0.8f, 0.8f, 2.8f, 3.8f, 1 },
        new float[7] { 2, 4, 0.94f, 0.8f, 2.2f, 3.2f, 1 },
        new float[7] { 1.7f, 3.7f, 1.08f, 0.8f, 2.2f, 3.2f, 2 },
        new float[7] { 1.3f, 3.3f, 1.22f, 0.8f, 2, 3, 2 },
        new float[7] { 1, 3, 1.36f, 0.8f, 2, 3, 2 }
    };
    // kitchen wait time, pickup time
    private float[][] waveKitchenData =
    {
        new float[2] { 15, 3 },
        new float[2] { 18, 0 },
        new float[2] { 20, 0 }
    };

    [SerializeField]
    private GameObject[] pastaSpawners;
    [SerializeField]
    private GameObject[] forcefieldSpawners;
    [SerializeField]
    private GameObject[] enemySpawners;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private UIManager uiManager;

    public void increaseKills()
    {
        kills++;

        int killQuota = (int)waveKillsData[findIndex(currentWave, waveKillsData.Length)];

        uiManager.updateKillsCounter(kills, killQuota);


        if (kills >= killQuota)
        {
            // spawn boss
            // stop spawning enemies

            // temporary solution:
            startNewWave();
        }
    }

    private int findIndex(int wave, int dataLength)
    {
        if (wave >= dataLength)
        {
            return dataLength - 1;
        }
        else
        {
            return wave;
        }
    }

    public void Start()
    {
        startNewWave();
    }

    public void startNewWave()
    {

        // update pasta spawners
        foreach (GameObject spawner in pastaSpawners)
        {
            Spawner spawnerScript = spawner.GetComponent<Spawner>();
            spawnerScript.updateBehavior(wavePastaData[findIndex(currentWave, wavePastaData.Length)]);
        }
        // update shield spawners
        foreach (GameObject spawner in forcefieldSpawners)
        {
            Spawner spawnerScript = spawner.GetComponent<Spawner>();
            spawnerScript.updateBehavior(waveShieldData[findIndex(currentWave, waveShieldData.Length)]);
        }
        // update enemies
        foreach (GameObject spawner in enemySpawners)
        {
            EnemySpawner spawnerScript = spawner.GetComponent<EnemySpawner>();
            spawnerScript.updateBehavior(waveEnemyData[findIndex(currentWave, waveEnemyData.Length)]);
        }
        // update kitchen
        Player playerScript = player.GetComponent<Player>();
        playerScript.updateKitchen(waveKitchenData[findIndex(currentWave, waveKitchenData.Length)]);

        Debug.Log("New Wave: " + (currentWave + 1));

        currentWave++;
        kills = 0;

        uiManager.updateKillsCounter(kills, (int)waveKillsData[findIndex(currentWave, waveKillsData.Length)]);

    }
}