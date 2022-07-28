using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kitchen : MonoBehaviour
{
    [SerializeField]
    private GameObject openDoor;
    public bool kitchenReady = true;

    // Start is called before the first frame update
    void Start()
    {
        openDoor.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && kitchenReady)
        {
            openDoor.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            openDoor.SetActive(false);
        }
    }
}
