using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freeze_Rotation : MonoBehaviour
{
    // Update is called once per frame
    void LateUpdate()
    {
        transform.eulerAngles = new Vector3(0, 0, 0);
    }
}
