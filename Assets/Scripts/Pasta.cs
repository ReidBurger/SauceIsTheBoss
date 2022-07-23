using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pasta : MonoBehaviour
{
    [SerializeField]
    private float initialSpeed = 14f;

    private float speed;
    [SerializeField]
    private AudioClip splat_sfx;
    private AudioSource source;
    private bool hasSplat = false;

    private void Start()
    {
        source = transform.GetComponent<AudioSource>();
        speed = initialSpeed;
        StartCoroutine(pastaShoot());
    }

    private IEnumerator pastaShoot()
    {
        transform.Translate(transform.up * speed * Time.deltaTime, Space.World);
        speed -= 0.16f;
        if (speed <= 0 && hasSplat == false)
        {
            hasSplat = true;
            source.PlayOneShot(splat_sfx, 0.5f);
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            Destroy(gameObject, 0.4f);
        }

        yield return new WaitForFixedUpdate();
        StartCoroutine(pastaShoot());
    }
}
