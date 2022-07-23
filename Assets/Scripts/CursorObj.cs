using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorObj : MonoBehaviour
{
    private void Start()
    {
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        transform.position = ray.origin;

        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }
}
