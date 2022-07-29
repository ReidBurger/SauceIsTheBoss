using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    private float runSpeed = 0;
    private float slowSpeed = 0;
    private float runTime = 0;
    private float fireRate = 0;
    private float accuracy = 0;
    private float slowTime = 0;
    [SerializeField]
    private GameObject bullet;
    private GameObject player;
    [SerializeField]
    private AudioClip shoot_sfx;
    [SerializeField]
    private AudioClip death_sfx;
    private AudioSource source;
    [SerializeField]
    private GameObject gun;
    private Pathfinding.AIPath aiPath;
    private bool canShoot = false;

    // Start is called before the first frame update
    void Start()
    {
        source = transform.GetComponent<AudioSource>();
        player = GameObject.FindWithTag("Player");
        aiPath = gameObject.GetComponent<Pathfinding.AIPath>();
        StartCoroutine(moveRoutine());
    }

    public void updateBehavior(float[] bossData)
    {
        runSpeed = bossData[0];
        runTime = bossData[1];
        slowSpeed = bossData[2];
        fireRate = bossData[3];
        accuracy = bossData[4];
        slowTime = bossData[5];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("pasta"))
        {
            GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            if (gameManager != null) gameManager.startNewWave();

            Destroy(collision.gameObject);
            enemyDie();
        }
    }

    private void enemyDie()
    {
        StopAllCoroutines();
        source.PlayOneShot(death_sfx, 1);
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        gameObject.GetComponent<Collider2D>().enabled = false;
        gun.SetActive(false);
        Destroy(gameObject, 0.5f);
    }

    private IEnumerator moveRoutine()
    {
        while (true)
        {
            aiPath.maxSpeed = runSpeed;
            yield return new WaitForSeconds(runTime);
            aiPath.maxSpeed = slowSpeed;
            canShoot = true;
            StartCoroutine(shootRoutine());
            yield return new WaitForSeconds(slowTime / 2);
            canShoot = false;
            yield return new WaitForSeconds(slowTime / 2);
        }
    }

    private IEnumerator shootRoutine()
    {
        if (player != null)
        {
            while (canShoot)
            {
                // worst miss angle from 0 to 90 based on the accuracy
                float maxMissAngle = Mathf.Abs(accuracy - 1) * 90;
                // returns random value from -worst to +worst
                float angleOffset = Random.Range(maxMissAngle * -1, maxMissAngle);

                transform.up = player.transform.position - transform.position;
                transform.Rotate(new Vector3(0, 0, angleOffset));

                source.PlayOneShot(shoot_sfx, 1);
                GameObject newBullet = Instantiate(bullet, transform.position, transform.rotation);

                yield return new WaitForSeconds(fireRate);
            }
        }
    }

    private void facePlayer()
    {
        Vector3 newPos = new Vector3(player.transform.position.x, player.transform.position.y, -9.7f);

        transform.LookAt(newPos, Vector3.forward);
        transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z * -1);

        if (transform.eulerAngles.z > 180)
        {
            // facing right
            gun.transform.localScale = new Vector2(8, 8);
        }
        else
        {
            // facing left
            gun.transform.localScale = new Vector2(8, -8);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null) facePlayer();
    }
}
