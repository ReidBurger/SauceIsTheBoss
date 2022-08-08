using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    private float upBound = 6;
    private float downBound = -6;
    private float rightBound = 11;
    private float leftBound = -11;
    [SerializeField]
    private AudioClip hitWall;
    private AudioSource audioSource;
    private bool hasMissed = false;
    public float sfx_volume = 1;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        bool isOnScreen = true;
        if (transform.position.x > rightBound || transform.position.x < leftBound
            || transform.position.y > upBound || transform.position.y < downBound
            || transform.position.z < 0 || transform.position.z > 0)
        { isOnScreen = false; }

        if (isOnScreen)
        {
            transform.Translate(transform.up * speed * Time.deltaTime, Space.World);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall") && !hasMissed)
        {
            hasMissed = true;
            speed = 0;
            if (hitWall != null && audioSource != null)
            {
                audioSource.PlayOneShot(hitWall, sfx_volume * 0.8f);
            }
            Destroy(gameObject, 0.2f);
            gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;
        }
    }
}
