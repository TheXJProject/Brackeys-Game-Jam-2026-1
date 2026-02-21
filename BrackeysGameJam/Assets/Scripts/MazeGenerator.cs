using System;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public class GameObjectSpawnInfo
{
    public GameObject gameObject;
    public int nodeIndex;
    public Maze.WallDirection wallFace;
}

public class MazeGenerator : MonoBehaviour
{
    public GameObject wallGameObject;
    public GameObject exitDoorGameObject;

    public Material lucidWallMaterial;
    public float lucidMazePercentInFloor;
    public string lucidMazeLayerName;

    private Quaternion northSouthRotation = Quaternion.Euler(0, 0, 0);
    private Quaternion eastWestRotation = Quaternion.Euler(0, 90, 0);

    public static event Action<Maze> onMazeGenerated;

    public List<int> mazeIndexes;
    private List<GameObject> generatedMazesObjects = new();

    public Maze selectedMaze;
    public List<GameObjectSpawnInfo> objectsToSpawn;

    public AsleepTrapManager trapManager;
    public AsleepEnemyManager enemyManager;
    public AsleepButtonManager buttonManager;

    private void Awake()
    {
        foreach (int mazeIndex in mazeIndexes)
        {
            Maze maze = Mazes.allMazes[mazeIndex];
            GameObject generatedMazeObject = CreateMaze(maze);
            generatedMazesObjects.Add(generatedMazeObject);
            generatedMazeObject.SetActive(false);
        }

        generatedMazesObjects[0].SetActive(true);
    }

    private void Start()
    {
        Maze maze = Mazes.allMazes[mazeIndexes[0]];

        enemyManager?.SpawnEnemies(maze);
        trapManager?.SpawnTraps(maze);
        buttonManager?.SpawnButtons(maze);

        SpawnGameObjects(maze);

        onMazeGenerated?.Invoke(maze);
    }

    private void SpawnGameObjects(Maze maze)
    {
        foreach (var objectInfo in objectsToSpawn)
        {
            int row = objectInfo.nodeIndex / maze.size;
            int col = objectInfo.nodeIndex % maze.size;

            Vector3 offsetVector = maze.getOnWallFaceOffset(objectInfo.wallFace);
            Vector3 rotationVector = maze.getOnWallFaceRotation(objectInfo.wallFace);

            Instantiate(objectInfo.gameObject,
                new Vector3(col * maze.scale.x, 0, -row * maze.scale.z) + offsetVector,
                Quaternion.Euler(rotationVector), transform);
        }
    }

    private (GameObject wallObject, GameObject lucidWallObject) CreateWall(int row, int col,
        Maze.WallDirection direction, Maze.WallType type, Maze maze)
    {
        GameObject wallObject = wallGameObject;
        switch (type)
        {
            case Maze.WallType.Wall:
                wallObject = Instantiate(wallGameObject);
                break;
            case Maze.WallType.ExitDoor:
                wallObject = Instantiate(exitDoorGameObject);
                break;
        }

        float wallX = col * maze.cellWidth;
        float wallY = maze.cellWidth / 2.0f;
        float wallZ = -row * maze.cellWidth;
        Quaternion wallRotation = new Quaternion();

        switch (direction)
        {
            case Maze.WallDirection.North:
                wallZ += maze.wallOffset;
                wallRotation = northSouthRotation;
                break;
            case Maze.WallDirection.South:
                wallZ -= maze.wallOffset;
                wallRotation = northSouthRotation;
                break;
            case Maze.WallDirection.West:
                wallX -= maze.wallOffset;
                wallRotation = eastWestRotation;
                break;
            case Maze.WallDirection.East:
                wallX += maze.wallOffset;
                wallRotation = eastWestRotation;
                break;
        }

        wallObject.name = $"{type} ({row}, {col}) {direction.ToString()}";

        wallObject.transform.localPosition = new Vector3(wallX, wallY, wallZ);
        wallObject.transform.localRotation = wallRotation;

        if (type == Maze.WallType.Wall)
        {
            GameObject lucidWallObject = Instantiate(wallObject);
            lucidWallObject.transform.localPosition =
                new Vector3(wallX, wallY - maze.cellWidth * lucidMazePercentInFloor, wallZ);
            lucidWallObject.transform.localRotation = wallRotation;
            lucidWallObject.layer = LayerMask.NameToLayer(lucidMazeLayerName);

            return (wallObject, lucidWallObject);
        }

        return (wallObject, null);
    }

    public GameObject CreateMaze(Maze maze)
    {
        GameObject mazeObject = new GameObject
        {
            name = "Maze",
            transform =
            {
                position = transform.localPosition,
                rotation = transform.rotation,
                localScale = transform.localScale,
                parent = transform
            },
        };

        // Parent GameObject for the lucid maze outline
        GameObject lucidMazeObject = new GameObject
        {
            name = "LucidMazeOutline",
            transform =
            {
                position = transform.localPosition,
                rotation = transform.rotation,
                localScale = transform.localScale,
                parent = mazeObject.transform
            },
        };

        GameObject navMeshObject = new GameObject
        {
            name = "NavMesh",
            transform =
            {
                position = transform.localPosition,
                rotation = transform.rotation,
                localScale = transform.localScale,
                parent = mazeObject.transform
            },
        };

        NavMeshSurface navMeshSurface = navMeshObject.AddComponent<NavMeshSurface>();
        navMeshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;

        maze.cellWidth = wallGameObject.transform.localScale.x - wallGameObject.transform.localScale.z;
        maze.wallOffset = maze.cellWidth / 2.0f;
        maze.SetupStartNodePosition(transform.localScale);

        foreach (var (node, connectingNodes) in maze.nodeConnections)
        {
            int row = node / maze.size;
            int col = node % maze.size;

            int northIndex = node - maze.size;
            int southIndex = node + maze.size;
            int westIndex = node - 1;
            int eastIndex = node + 1;

            (int index, Maze.WallDirection direction, Func<int, bool> edgeCheck)[] indexDirectionMapping =
            {
                (northIndex, Maze.WallDirection.North, (idx) => idx < 0),
                (southIndex, Maze.WallDirection.South, (idx) => idx > (maze.size * maze.size - 1)),
                (westIndex, Maze.WallDirection.West, (idx) => idx / maze.size != row),
                (eastIndex, Maze.WallDirection.East, (idx) => idx / maze.size != row),
            };

            foreach (var (index, direction, edgeCheck) in indexDirectionMapping)
            {
                bool isEdge = edgeCheck(index);

                if (node == maze.endNode.nodeIndex && direction == maze.endNode.direction)
                {
                    var (exitDoorInstance, _) =
                        CreateWall(row, col, direction, Maze.WallType.ExitDoor, maze);

                    exitDoorInstance.transform.SetParent(mazeObject.transform, false);
                    continue;
                }

                if (!connectingNodes.Contains(index) && (index < node || isEdge))
                {
                    var (wallInstance, lucidWallInstance) =
                        CreateWall(row, col, direction, Maze.WallType.Wall, maze);

                    wallInstance.transform.SetParent(mazeObject.transform, false);
                    lucidWallInstance.transform.SetParent(lucidMazeObject.transform, false);
                }
            }
        }

        navMeshSurface.BuildNavMesh();
        return mazeObject;
    }
}
