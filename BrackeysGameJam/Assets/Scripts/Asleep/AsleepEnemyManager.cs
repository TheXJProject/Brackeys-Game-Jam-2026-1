using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemySpawnInfo
{
    public int startNodeIndex;
    public List<int> allowedTargetNodes;
}

public class AsleepEnemyManager : MonoBehaviour
{
    public GameObject enemyGameObject;
    public List<EnemySpawnInfo> enemiesToSpawn;

    public void SpawnEnemies(Maze maze)
    {
        int enemyCount = 0;

        foreach (var enemy in enemiesToSpawn)
        {
            int row = enemy.startNodeIndex / maze.size;
            int col = enemy.startNodeIndex % maze.size;

            GameObject newEnemy = Instantiate(enemyGameObject,
                new Vector3(col * maze.scale.x, 0, -row * maze.scale.z), Quaternion.identity, transform);

            AsleepEnemy enemyScript = newEnemy.GetComponent<AsleepEnemy>();

            enemyScript.SetAllowedTargetNodes(enemy.allowedTargetNodes);
            enemyScript.enemyID = enemyCount++;
        }
    }
}
