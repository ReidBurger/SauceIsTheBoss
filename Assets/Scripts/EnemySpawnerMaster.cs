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
    [SerializeField]
    private GameObject shatterParticles;

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

            switch (spawnersActive)
            {
                case 1:
                    Instantiate(shatterParticles, new Vector2(-4.1f, 3.5f), Quaternion.Euler(0, 0, 0));
                    break;
                case 2:
                    Instantiate(shatterParticles, new Vector2(-9.2f, 1.4f), Quaternion.Euler(0, 0, 90));
                    break;
                case 3:
                    Instantiate(shatterParticles, new Vector2(7.8f, 3.5f), Quaternion.Euler(0, 0, 0));
                    break;
                default:
                    break;
            }

            if (_audioSource != null)
            {
                _audioSource.PlayOneShot(shatter, 1);
            }
        }
    }
}
