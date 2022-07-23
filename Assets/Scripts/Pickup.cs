using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField]
    private Sprite normal;
    [SerializeField]
    private Sprite shadow;
    private SpriteRenderer image;

    private void Start()
    {
        image = GetComponent<SpriteRenderer>();

        image.sprite = normal;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            image.sprite = shadow;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            image.sprite = normal;
        }
    }
}
