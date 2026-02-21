using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TrapSpawnInfo
{
    public int spawnNodeIndex;
}

public class AsleepTrapManager : MonoBehaviour
{
    public GameObject trapGameObject;
    public List<TrapSpawnInfo> trapsToSpawn;
    public int numberOfTrapsRequired;
    public float trapYPosition;

    private int trapActivated;

    public static event Action onAllTrapsActivated;

    private void OnEnable()
    {
        AsleepTrap.onEnemyTrapped += EnemyTrapped;
    }

    private void OnDisable()
    {
        AsleepTrap.onEnemyTrapped -= EnemyTrapped;
    }

    private void Start()
    {
        AsleepInteractable.onPuzzlePieceAdded?.Invoke();
    }

    private void EnemyTrapped(GameObject enemy)
    {
        trapActivated++;

        if (trapActivated >= numberOfTrapsRequired)
        {
            onAllTrapsActivated?.Invoke();
        }
    }

    public void SpawnTraps(Maze maze)
    {
        foreach (TrapSpawnInfo trapSpawnInfo in trapsToSpawn)
        {
            int row = trapSpawnInfo.spawnNodeIndex / maze.size;
            int col = trapSpawnInfo.spawnNodeIndex % maze.size;

            Instantiate(trapGameObject,
                new Vector3(col * maze.scale.x * maze.cellWidth, trapYPosition, -row * maze.scale.z * maze.cellWidth),
                Quaternion.identity, transform);
        }
    }
}
