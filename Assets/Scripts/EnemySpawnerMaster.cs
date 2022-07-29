using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerMaster : MonoBehaviour
{
    [SerializeField]
    private GameObject[] enemySpawners;
    private int spawnersActive = 0;
    private AudioSource _audioSource;
    [SerializeField]
    private AudioClip shatter;

    // order in array is order they will activate

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void updateEnemySpawners(float openNextDoor)
    {
        if (openNextDoor == 1)
        {
            enemySpawners[spawnersActive].SetActive(true);
            spawnersActive++;

            if (_audioSource != null)
            {
                _audioSource.PlayOneShot(shatter, 1);
            }
        }
    }
}
