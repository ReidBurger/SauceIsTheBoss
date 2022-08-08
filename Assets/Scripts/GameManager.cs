using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        new float[3] { 4, 6, 5 },
        new float[3] { 4.2f, 6.4f, 4 },
        new float[3] { 4, 5.8f, 4 },
        new float[3] { 4.2f, 6, 4 },
        new float[3] { 3, 4, 4 }

    };
    // shield min spawn time, max spawn time, active time
    private float[][] waveShieldData =
    {
        new float[3] { 18, 20, 9.5f },
        new float[3] { 22, 28, 8.5f },
        new float[3] { 25, 30, 8 },
        new float[3] { 28, 32, 7.8f },
        new float[3] { 26, 30, 8 }
    };
    // enemy min spawn time, max spawn time, speed, accuracy, shoot time min,
    // shoot time max, max enemies on screen per spawner, open new door
    private float[][] waveEnemyData =
    {
        new float[8] { 1.8f, 2.8f, 0.8f, 0.8f, 1.6f, 2.5f, 1, 1 },
        new float[8] { 3.5f, 4.5f, 0.85f, 0.8f, 2, 3, 1, 1 },
        new float[8] { 3, 4, 0.9f, 0.8f, 2.2f, 3.2f, 2, 0 },
        new float[8] { 3.2f, 4.2f, 0.95f, 0.8f, 2, 3, 1, 1 },
        new float[8] { 3.4f, 4.6f, 1, 0.8f, 2, 3, 2, 0 }
    };
    // kitchen wait time, pickup time
    private float[][] waveKitchenData =
    {
        new float[2] { 15, 2 },
        new float[2] { 14, 2 },
        new float[2] { 12, 2 },
        new float[2] { 10, 2 },
        new float[2] { 8, 2 }
    };
    // boss run speed, run time, slow speed, fire rate, accuracy, slow time
    private float[][] waveBossData =
    {
        new float[6] { 2.2f, 1.3f, 0, 0.35f, 0.7f, 2 },
        new float[6] { 2.3f, 1.3f, 0.2f, 0.3f, 0.7f, 2 },
        new float[6] { 2.4f, 1.3f, 0.4f, 0.25f, 0.7f, 2 },
        new float[6] { 2.6f, 1.2f, 0.5f, 0.25f, 0.7f, 2 }
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
    [SerializeField]
    private SpawnCounter _spawnCounter;
    [SerializeField]
    private EnemySpawnerMaster _enemySpawnMaster;
    [SerializeField]
    private AudioSource preGame;
    [SerializeField]
    private AudioSource game;
    [SerializeField]
    private AudioSource postGame;
    [SerializeField]
    private GameObject pauseMenu;
    [SerializeField]
    private GameObject startScreen;
    [SerializeField]
    private GameObject gameOverScreen;

    public void Start()
    {
        updateSettings();

        if (player != null)
        {
            Player.PlayerDeath += onGameOver;
            Enemy.KitchenReached += onGameOver;
            Boss.KitchenReached += onGameOver;
        }

        if (postGame.isPlaying) postGame.Stop();
        preGame.Play();

        // if scene is game, start new wave

        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Game"))
        {
            preGame.Stop();
            game.Play();
            startNewWave();
        }
    }

    public void updateSettings()
    {
        float setSfxVol = MainManager.Instance.sfx_vol / 10f;
        // get info from MainManager and update all music, sfx, movement
        if (player != null)
        {
            Player p = player.GetComponent<Player>();
            p.sfx_volume = setSfxVol;
            p.instantAcceleration = MainManager.Instance.instant_acceleration;
        }

        if (_enemySpawnMaster != null)
        {
            EnemySpawnerMaster esm = _enemySpawnMaster.GetComponent<EnemySpawnerMaster>();
            esm.sfx_volume = setSfxVol;
            esm.updateSpawnerSFX();
            BossManager bm = _enemySpawnMaster.GetComponent<BossManager>();
            bm.sfx_volume = setSfxVol;
        }

        float setMusicVol = MainManager.Instance.music_vol / 10f;
        postGame.volume = setMusicVol;
        game.volume = setMusicVol;
        preGame.volume = setMusicVol;
        //Debug.Log("Update: " + setSfxVol + "|" + setMusicVol);
    }

    private void pauseGame()
    {
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
        if (player != null)
        {
            Player p = player.GetComponent<Player>();
            p.cursorObj.SetActive(false);
        }
        if (startScreen != null)
        {
            startScreen.SetActive(false);
        }
        if (gameOverScreen != null && gameOverScreen.activeSelf)
        {
            gameOverScreen.SetActive(false);
        }
    }

    private void unpauseGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        if (startScreen != null)
        {
            startScreen.SetActive(true);
        }
        else
        {
            // if there isn't a start screen, then it must be "game", so unhide the cursor
            if (player != null)
            {
                Player p = player.GetComponent<Player>();
                p.cursorObj.SetActive(true);
            }

            if (player == null)
            {
                // player has died
                gameOverScreen.SetActive(true);
            }
        }
    }

    public void loadGame()
    {
        SceneManager.LoadScene(0);
    }

    public void increaseKills()
    {
        kills++;

        int killQuota = (int)waveKillsData[findIndex(currentWave, waveKillsData.Length)];

        uiManager.updateKillsCounter(kills, killQuota);


        if (kills >= killQuota)
        {
            EnemySpawner firstSpawner = enemySpawners[0].GetComponent<EnemySpawner>();
            if (!firstSpawner.bossOut)
            {
                foreach (GameObject _enemySpawner in enemySpawners)
                {
                    EnemySpawner enemySpawner = _enemySpawner.GetComponent<EnemySpawner>();
                    enemySpawner.bossOut = true;
                }

                BossManager _bossManager = _enemySpawnMaster.GetComponent<BossManager>();
                _bossManager.spawnBoss();
            }
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

    private void onGameOver()
    {
        game.Stop();
        postGame.Play();

        player.SetActive(false);

        int thrown = 0;
        Player _player = player.GetComponent<Player>();
        if (_player != null)
        {
            thrown = _player.totalThrown;
        }

        int totalKills = kills;
        for (int wave = 0; wave < currentWave - 1; wave++)
        {
            totalKills += (int)waveKillsData[findIndex(wave, waveKillsData.Length)];
        }

        uiManager.displayGameOver(true, totalKills, thrown);
    }

    public void startNewWave()
    {
        foreach (GameObject _enemySpawner in enemySpawners)
        {
            EnemySpawner enemySpawner = _enemySpawner.GetComponent<EnemySpawner>();
            enemySpawner.bossOut = false;
        }

        // update pasta spawners
        foreach (GameObject spawner in pastaSpawners)
        {
            Spawner spawnerScript = spawner.GetComponent<Spawner>();
            spawnerScript.updateBehavior(wavePastaData[findIndex(currentWave, wavePastaData.Length)]);
        }
        // update pasta spawner counter
        SpawnCounter spawnCounter = _spawnCounter.GetComponent<SpawnCounter>();
        spawnCounter.updateSpawnCount(wavePastaData[findIndex(currentWave, wavePastaData.Length)][2]);
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
        //update enemy spawner activator
        EnemySpawnerMaster enemySpawnerMaster = _enemySpawnMaster.GetComponent<EnemySpawnerMaster>();
        enemySpawnerMaster.updateEnemySpawners(waveEnemyData[findIndex(currentWave, waveEnemyData.Length)][7]);
        // update kitchen
        Player playerScript = player.GetComponent<Player>();
        playerScript.updateKitchen(waveKitchenData[findIndex(currentWave, waveKitchenData.Length)]);
        // update boss manager
        BossManager _bossManager = _enemySpawnMaster.GetComponent<BossManager>();
        _bossManager.updateBehavior(waveBossData[findIndex(currentWave, waveBossData.Length)]);

        uiManager.roundDisplay(currentWave + 1);

        currentWave++;
        kills = 0;

        uiManager.updateKillsCounter(kills, (int)waveKillsData[findIndex(currentWave, waveKillsData.Length)]);

    }

    private void OnDestroy()
    {
        Player.PlayerDeath -= onGameOver;
        Enemy.KitchenReached -= onGameOver;
        Boss.KitchenReached -= onGameOver;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Time.timeScale != 0)
            {
                pauseGame();
            }
            else
            {
                unpauseGame();
            }
        }
    }
}
