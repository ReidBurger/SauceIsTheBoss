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
}
