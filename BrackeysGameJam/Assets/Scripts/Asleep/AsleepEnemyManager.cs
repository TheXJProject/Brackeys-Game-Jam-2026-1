using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyLocation
{
    public int row;
    public int col;
}

public class AsleepEnemyManager : MonoBehaviour
{
    public MazeGenerator mazeGenerator;
    public GameObject enemy;
    public List<EnemyLocation> enemySpawnLocations;

    void Start()
    {
        foreach (var enemyLocation in enemySpawnLocations)
        {
            Instantiate(enemy,
                new Vector3(enemyLocation.col * mazeGenerator.cellWidth, 0,
                    -enemyLocation.row * mazeGenerator.cellWidth), Quaternion.identity, transform);
        }
    }
}
