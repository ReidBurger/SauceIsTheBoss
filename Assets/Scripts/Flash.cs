using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flash : MonoBehaviour
{
    [SerializeField]
    private float rate = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Blink", rate, rate);
    }

    private void Blink()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
}
