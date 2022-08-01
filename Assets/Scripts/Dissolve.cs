using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
    [SerializeField]
    private float time = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, time);
    }
}
