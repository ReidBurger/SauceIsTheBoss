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
    [SerializeField]
    private AudioClip shoot_sfx;
    [SerializeField]
    private AudioClip death_sfx;
    private AudioSource source;
    [SerializeField]
    private GameObject gun;
    [SerializeField]
    private Animator animator;
    private Vector2 previousPosition;
    private Vector2 currentPosition;
    private GameObject kitchen;

    // Start is called before the first frame update
    void Start()
    {
        source = transform.GetComponent<AudioSource>();
        player = GameObject.FindWithTag("Player");
        kitchen = GameObject.FindWithTag("Target");
        previousPosition = transform.position;
        InvokeRepeating("updateAnimation", 0, 0.5f);
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
        StopAllCoroutines();
        Pathfinding.AIPath aiPath = gameObject.GetComponent<Pathfinding.AIPath>();
        aiPath.maxSpeed = 0;
        source.PlayOneShot(death_sfx, 1);
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        gameObject.GetComponent<Collider2D>().enabled = false;
        gun.SetActive(false);
        Destroy(gameObject, 0.5f);
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

            source.PlayOneShot(shoot_sfx, 1);
            GameObject newBullet = Instantiate(bullet, transform.position, transform.rotation);

            StartCoroutine(shootRoutine());
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

    private void faceKitchen()
    {
        Vector3 newPos = new Vector3(kitchen.transform.position.x, kitchen.transform.position.y, -9.7f);

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

    private void updateAnimation()
    {
        currentPosition = transform.position;
        Vector2 velocity = currentPosition - previousPosition;

        animator.SetFloat("Horizontal_Velocity", velocity.x);
        animator.SetFloat("Vertical_Velocity", velocity.y);

        //Debug.Log(velocity.x + " " + velocity.y);

        previousPosition = currentPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null) facePlayer();
        else
        {
            faceKitchen();
        }
        if (Vector2.Distance(transform.position, kitchen.transform.position) < 0.1f)
        {
            // Game Over
            Destroy(gameObject);
        }
    }
}
