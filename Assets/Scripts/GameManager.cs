using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int kills = 0;
    private int currentWave = 0;

    // array index corresponds to wave number for the following

    // kills to spawn boss
    private float[] waveKillsData = { 10, 15, 19, 24 };

    // pasta min spawn time, max spawn time, pasta on screen
    private float[][] wavePastaData =
    {
        new float[3] { 1.2f, 3.2f, 8 },
        new float[3] { 1.5f, 3.5f, 7 },
        new float[3] { 1.8f, 3.8f, 6 },
        new float[3] { 2.1f, 4.1f, 6 },
        new float[3] { 2.3f, 4.3f, 5 }

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
        new float[7] { 2, 4, 0.85f, 0.8f, 2.2f, 3.2f, 1 },
        new float[7] { 2.2f, 4.2f, 0.9f, 0.8f, 2.2f, 3.2f, 2 },
        new float[7] { 1.9f, 3.5f, 0.95f, 0.8f, 2, 3, 2 },
        new float[7] { 1, 3, 1, 0.8f, 2, 3, 2 }
    };
    // kitchen wait time, pickup time
    private float[][] waveKitchenData =
    {
        new float[2] { 15, 1.5f },
        new float[2] { 18, 1.5f },
        new float[2] { 20, 1.5f }
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

        uiManager.roundDisplay(currentWave + 1);

        currentWave++;
        kills = 0;

        uiManager.updateKillsCounter(kills, (int)waveKillsData[findIndex(currentWave, waveKillsData.Length)]);

    }
}
