using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    private float upBound = 10;
    private float downBound = -10;
    private float rightBound = 10;
    private float leftBound = -10;

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
