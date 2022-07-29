using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCounter : MonoBehaviour
{
    public int currentTotal = 0;
    public int maxSpawnCount = 10;

    public void updateSpawnCount(float max)
    {
        maxSpawnCount = (int)max;
    }
}
