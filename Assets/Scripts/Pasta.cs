using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pasta : MonoBehaviour
{
    [SerializeField]
    private float initialSpeed = 12f;

    private float speed;

    private void Start()
    {
        speed = initialSpeed;
        StartCoroutine(pastaShoot());
    }

    private IEnumerator pastaShoot()
    {
        transform.Translate(transform.up * speed * Time.deltaTime, Space.World);
        speed -= 0.18f;
        if (speed <= 0) Destroy(gameObject);

        yield return new WaitForFixedUpdate();
        StartCoroutine(pastaShoot());
    }
}
