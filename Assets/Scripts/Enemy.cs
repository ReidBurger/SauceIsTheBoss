using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float minWaitTime = 1.8f;
    private float maxWaitTime = 2.8f;
    // accuracy is a value from 0 to 1, 0 is wild, 1 is 100% accurate
    private float accuracy = 0.82f;
    [SerializeField]
    private GameObject bullet;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        StartCoroutine(shootRoutine());
    }

    public void updateBehavior(float shootMinTime, float shootMaxTime, float enemyAccuracy, float enemySpeed)
    {
        minWaitTime = shootMinTime;
        maxWaitTime = shootMaxTime;
        accuracy = enemyAccuracy;

        Pathfinding.AIPath aiPath = gameObject.GetComponent<Pathfinding.AIPath>();
        aiPath.maxSpeed = enemySpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("pasta"))
        {
            GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            if (gameManager != null) gameManager.increaseKills();

            Destroy(collision.gameObject);
            enemyDie();
        }
    }

    private void enemyDie()
    {
        Destroy(gameObject);
    }

    private IEnumerator shootRoutine()
    {
        yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));

        if (player != null)
        {
            // worst miss angle from 0 to 90 based on the accuracy
            float maxMissAngle = Mathf.Abs(accuracy - 1) * 90;
            // returns random value from -worst to +worst
            float angleOffset = Random.Range(maxMissAngle * -1, maxMissAngle);

            transform.up = player.transform.position - transform.position;
            transform.Rotate(new Vector3(0, 0, angleOffset));

            Instantiate(bullet, transform.position, transform.rotation);

            StartCoroutine(shootRoutine());
        }
    }

    private void facePlayer()
    {
        Vector3 newPos = new Vector3(player.transform.position.x, player.transform.position.y, -9.7f);

        transform.LookAt(newPos, Vector3.forward);
        transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z * -1);
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null) facePlayer();
    }
}
