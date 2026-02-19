using System;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

[Serializable]
public class GameObjectSpawnInfo
{
    public GameObject gameObject;
    public int nodeIndex;
}

[Serializable]
public class EnemySpawnInfo
{
    public GameObject gameObject;
    public int startNodeIndex;
    public List<int> allowedTargetNodes;
}

public class Maze
{
    public enum WallDirection
    {
        North,
        South,
        East,
        West
    }

    public enum WallType
    {
        Wall,
        ExitDoor
    }

    public Dictionary<int, HashSet<int>> nodeConnections;
    public int size;
    public (int nodeIndex, WallDirection direction) startNode;
    public (int nodeIndex, WallDirection direction) endNode;
    public float cellWidth;
    public Vector3 startNodePosition;
    public Vector3 scale;

    public Maze(Dictionary<int, HashSet<int>> nodeConnections, int size,
        (int nodeIndex, WallDirection direction) startNode,
        (int nodeIndex, WallDirection direction) endNode)
    {
        this.nodeConnections = nodeConnections;
        this.size = size;
        this.startNode = startNode;
        this.endNode = endNode;
    }

    public void SetupStartNodePosition(Vector3 mazeScale)
    {
        int startNodeRow = startNode.nodeIndex / size;
        int startNodeCol = startNode.nodeIndex % size;

        scale = mazeScale;

        startNodePosition =
            new Vector3(startNodeCol * cellWidth * mazeScale.x, 0, -startNodeRow * cellWidth * mazeScale.z);
    }
}

public class MazeGenerator : MonoBehaviour
{
    public NavMeshSurface surface;

    public GameObject wallGameObject;
    public GameObject exitDoorGameObject;

    private GameObject lucidMazeOutline;
    public Material lucidWallMaterial;
    public float lucidMazePercentInFloor;
    public string lucidMazeLayerName;

    private float wallOffset;
    private Quaternion northSouthRotation = Quaternion.Euler(0, 0, 0);
    private Quaternion eastWestRotation = Quaternion.Euler(0, 90, 0);

    public static event Action<Maze> onMazeGenerated;

    public Maze selectedMaze;
    public List<EnemySpawnInfo> enemiesToSpawn;
    public List<GameObjectSpawnInfo> objectsToSpawn;

    // TODO: Load maze from file
    public static Maze maze_1 = new(nodeConnections: new Dictionary<int, HashSet<int>>
        {
            { 0, new HashSet<int> { 10 } },
            { 1, new HashSet<int> { 2 } },
            { 2, new HashSet<int> { 1, 3 } },
            { 3, new HashSet<int> { 2, 4 } },
            { 4, new HashSet<int> { 3, 5 } },
            { 5, new HashSet<int> { 4, 6, 15 } },
            { 6, new HashSet<int> { 5, 7 } },
            { 7, new HashSet<int> { 6, 8 } },
            { 8, new HashSet<int> { 7, 9 } },
            { 9, new HashSet<int> { 8, 19 } },
            { 10, new HashSet<int> { 0, 11 } },
            { 11, new HashSet<int> { 10, 12 } },
            { 12, new HashSet<int> { 11, 13, 22 } },
            { 13, new HashSet<int> { 12, 14 } },
            { 14, new HashSet<int> { 13, 24 } },
            { 15, new HashSet<int> { 5, 25 } },
            { 16, new HashSet<int> { 17 } },
            { 17, new HashSet<int> { 16, 18, 27 } },
            { 18, new HashSet<int> { 17, 28 } },
            { 19, new HashSet<int> { 9 } },
            { 20, new HashSet<int> { 30, 21 } },
            { 21, new HashSet<int> { 20 } },
            { 22, new HashSet<int> { 12, 23 } },
            { 23, new HashSet<int> { 22, 33 } },
            { 24, new HashSet<int> { 14, 34 } },
            { 25, new HashSet<int> { 15, 26, 35 } },
            { 26, new HashSet<int> { 25, 36 } },
            { 27, new HashSet<int> { 17, 37 } },
            { 28, new HashSet<int> { 18, 29 } },
            { 29, new HashSet<int> { 28, 39 } },
            { 30, new HashSet<int> { 20, 40 } },
            { 31, new HashSet<int> { 32, 41 } },
            { 32, new HashSet<int> { 31, 42 } },
            { 33, new HashSet<int> { 23, 43 } },
            { 34, new HashSet<int> { 24, 44 } },
            { 35, new HashSet<int> { 25 } },
            { 36, new HashSet<int> { 26, 46 } },
            { 37, new HashSet<int> { 27, 38 } },
            { 38, new HashSet<int> { 37, 48 } },
            { 39, new HashSet<int> { 29, 49 } },
            { 40, new HashSet<int> { 30, 50 } },
            { 41, new HashSet<int> { 31 } },
            { 42, new HashSet<int> { 32, 43 } },
            { 43, new HashSet<int> { 33, 42 } },
            { 44, new HashSet<int> { 34, 45, 54 } },
            { 45, new HashSet<int> { 44, 46 } },
            { 46, new HashSet<int> { 36, 45, 47 } },
            { 47, new HashSet<int> { 46, 57 } },
            { 48, new HashSet<int> { 38, 58 } },
            { 49, new HashSet<int> { 39, 59 } },
            { 50, new HashSet<int> { 40, 60, 51 } },
            { 51, new HashSet<int> { 50, 52, 61 } },
            { 52, new HashSet<int> { 51, 53 } },
            { 53, new HashSet<int> { 52, 54 } },
            { 54, new HashSet<int> { 53, 44 } },
            { 55, new HashSet<int> { 65, 56 } },
            { 56, new HashSet<int> { 55 } },
            { 57, new HashSet<int> { 47, 67, 58 } },
            { 58, new HashSet<int> { 48, 57, 68 } },
            { 59, new HashSet<int> { 49 } },
            { 60, new HashSet<int> { 50, 70 } },
            { 61, new HashSet<int> { 51, 71 } },
            { 62, new HashSet<int> { 72, 63 } },
            { 63, new HashSet<int> { 62, 64, 73 } },
            { 64, new HashSet<int> { 63, 65 } },
            { 65, new HashSet<int> { 55, 75, 64 } },
            { 66, new HashSet<int> { 67, 76 } },
            { 67, new HashSet<int> { 66, 57 } },
            { 68, new HashSet<int> { 58, 69, 78 } },
            { 69, new HashSet<int> { 68, 79 } },
            { 70, new HashSet<int> { 60, 80 } },
            { 71, new HashSet<int> { 61, 81 } },
            { 72, new HashSet<int> { 62 } },
            { 73, new HashSet<int> { 63, 83 } },
            { 74, new HashSet<int>() },
            { 75, new HashSet<int> { 65, 85 } },
            { 76, new HashSet<int> { 66, 77, 86 } },
            { 77, new HashSet<int> { 76, 78 } },
            { 78, new HashSet<int> { 77, 68 } },
            { 79, new HashSet<int> { 69, 89 } },
            { 80, new HashSet<int> { 70, 90 } },
            { 81, new HashSet<int> { 71, 82 } },
            { 82, new HashSet<int> { 81 } },
            { 83, new HashSet<int> { 73, 84, 93 } },
            { 84, new HashSet<int> { 83, 85 } },
            { 85, new HashSet<int> { 75, 84, 86 } },
            { 86, new HashSet<int> { 85, 76 } },
            { 87, new HashSet<int> { 97, 88 } },
            { 88, new HashSet<int> { 87, 98 } },
            { 89, new HashSet<int> { 79 } },
            { 90, new HashSet<int> { 80, 91 } },
            { 91, new HashSet<int> { 90, 92 } },
            { 92, new HashSet<int> { 91, 93 } },
            { 93, new HashSet<int> { 92, 83, 94 } },
            { 94, new HashSet<int> { 93, 95 } },
            { 95, new HashSet<int> { 94, 96 } },
            { 96, new HashSet<int> { 95, 97 } },
            { 97, new HashSet<int> { 96, 87 } },
            { 98, new HashSet<int> { 88, 99 } },
            { 99, new HashSet<int> { 98 } },
        },
        size: 10,
        startNode: (93, Maze.WallDirection.South),
        endNode: (0, Maze.WallDirection.North)
    );

    public static Maze maze_2 = new(nodeConnections: new Dictionary<int, HashSet<int>>
        {
            { 0, new HashSet<int> { 8 } },
            { 1, new HashSet<int> { 9, 2 } },
            { 2, new HashSet<int> { 1, 3 } },
            { 3, new HashSet<int> { 2, 4, 11 } },
            { 4, new HashSet<int> { 3, 5 } },
            { 5, new HashSet<int> { 4, 6 } },
            { 6, new HashSet<int> { 5, 7 } },
            { 7, new HashSet<int> { 6, 15 } },
            { 8, new HashSet<int> { 0, 9 } },
            { 9, new HashSet<int> { 8, 1, 10 } },
            { 10, new HashSet<int> { 9, 18 } },
            { 11, new HashSet<int> { 3, 19 } },
            { 12, new HashSet<int> { 20, 13 } },
            { 13, new HashSet<int> { 12, 21, 14 } },
            { 14, new HashSet<int> { 13, 15 } },
            { 15, new HashSet<int> { 7, 14, 23 } },
            { 16, new HashSet<int> { 24, 17 } },
            { 17, new HashSet<int> { 16, 25, 18 } },
            { 18, new HashSet<int> { 17, 10, 26 } },
            { 19, new HashSet<int> { 11, 27 } },
            { 20, new HashSet<int> { 12 } },
            { 21, new HashSet<int> { 13, 29 } },
            { 22, new HashSet<int> { 30, 23 } },
            { 23, new HashSet<int> { 15, 22 } },
            { 24, new HashSet<int> { 16, 32 } },
            { 25, new HashSet<int> { 17, 33 } },
            { 26, new HashSet<int> { 18, 34 } },
            { 27, new HashSet<int> { 19, 28 } },
            { 28, new HashSet<int> { 27, 29 } },
            { 29, new HashSet<int> { 28, 21 } },
            { 30, new HashSet<int> { 22, 31 } },
            { 31, new HashSet<int> { 30 } },
            { 32, new HashSet<int> { 24, 40 } },
            { 33, new HashSet<int> { 25, 41 } },
            { 34, new HashSet<int> { 26, 35 } },
            { 35, new HashSet<int> { 34, 36 } },
            { 36, new HashSet<int> { 35, 37, 44 } },
            { 37, new HashSet<int> { 36, 38 } },
            { 38, new HashSet<int> { 37, 39 } },
            { 39, new HashSet<int> { 38, 47 } },
            { 40, new HashSet<int> { 32 } },
            { 41, new HashSet<int> { 33, 49 } },
            { 42, new HashSet<int> { 50, 43 } },
            { 43, new HashSet<int> { 42, 44 } },
            { 44, new HashSet<int> { 43, 36 } },
            { 45, new HashSet<int> { 53, 46 } },
            { 46, new HashSet<int> { 45 } },
            { 47, new HashSet<int> { 39, 55 } },
            { 48, new HashSet<int> { 56, 49 } },
            { 49, new HashSet<int> { 48, 41 } },
            { 50, new HashSet<int> { 42, 58 } },
            { 51, new HashSet<int> { 59, 52 } },
            { 52, new HashSet<int> { 51, 53 } },
            { 53, new HashSet<int> { 52, 45, 54 } },
            { 54, new HashSet<int> { 53, 62 } },
            { 55, new HashSet<int> { 47, 63 } },
            { 56, new HashSet<int> { 48, 57 } },
            { 57, new HashSet<int> { 56, 58 } },
            { 58, new HashSet<int> { 57, 50, 59 } },
            { 59, new HashSet<int> { 58, 51 } },
            { 60, new HashSet<int> { 61 } },
            { 61, new HashSet<int> { 60, 62 } },
            { 62, new HashSet<int> { 61, 54, 63 } },
            { 63, new HashSet<int> { 62, 55 } },
        },
        size: 8,
        startNode: (60, Maze.WallDirection.South),
        endNode: (31, Maze.WallDirection.East)
    );

    private void Awake()
    {
        Maze[] mazes = { maze_1, maze_2 };
        selectedMaze = mazes[0];

        selectedMaze.cellWidth = wallGameObject.transform.localScale.x - wallGameObject.transform.localScale.z;
        wallOffset = selectedMaze.cellWidth / 2.0f;

        selectedMaze.SetupStartNodePosition(transform.localScale);
        CreateMaze(mazes[0]);
    }

    private void Start()
    {
        SpawnEnemies();
        SpawnGameObjects();

        onMazeGenerated?.Invoke(selectedMaze);
    }

    private void SpawnEnemies()
    {
        foreach (var enemy in enemiesToSpawn)
        {
            int row = enemy.startNodeIndex / selectedMaze.size;
            int col = enemy.startNodeIndex % selectedMaze.size;

            GameObject newEnemy = Instantiate(enemy.gameObject,
                new Vector3(col * selectedMaze.scale.x, 0, -row * selectedMaze.scale.z), Quaternion.identity);

            newEnemy.GetComponent<AsleepEnemy>().SetAllowedTargetNodes(enemy.allowedTargetNodes);
        }
    }

    private void SpawnGameObjects()
    {
        foreach (var objectInfo in objectsToSpawn)
        {
            int row = objectInfo.nodeIndex / selectedMaze.size;
            int col = objectInfo.nodeIndex % selectedMaze.size;

            Instantiate(objectInfo.gameObject,
                new Vector3(col * selectedMaze.cellWidth, 0, -row * selectedMaze.cellWidth), Quaternion.identity);
        }
    }

    private void CreateWall(int row, int col, Maze.WallDirection direction, Maze.WallType type)
    {
        GameObject wallObject = wallGameObject;
        switch (type)
        {
            case Maze.WallType.Wall:
                wallObject = Instantiate(wallGameObject, transform);
                break;
            case Maze.WallType.ExitDoor:
                wallObject = Instantiate(exitDoorGameObject, transform);
                break;
        }

        float wallX = col * selectedMaze.cellWidth;
        float wallY = selectedMaze.cellWidth / 2.0f;
        float wallZ = -row * selectedMaze.cellWidth;
        Quaternion wallRotation = new Quaternion();

        switch (direction)
        {
            case Maze.WallDirection.North:
                wallZ += wallOffset;
                wallRotation = northSouthRotation;
                break;
            case Maze.WallDirection.South:
                wallZ -= wallOffset;
                wallRotation = northSouthRotation;
                break;
            case Maze.WallDirection.West:
                wallX -= wallOffset;
                wallRotation = eastWestRotation;
                break;
            case Maze.WallDirection.East:
                wallX += wallOffset;
                wallRotation = eastWestRotation;
                break;
        }

        wallObject.name = $"{type} ({row}, {col}) {direction.ToString()}";

        wallObject.transform.localPosition = new Vector3(wallX, wallY, wallZ);
        wallObject.transform.localRotation = wallRotation;

        if (type == Maze.WallType.Wall)
        {
            GameObject lucidWallObject = Instantiate(wallObject, lucidMazeOutline.transform);
            lucidWallObject.transform.localPosition =
                new Vector3(wallX, wallY - selectedMaze.cellWidth * lucidMazePercentInFloor, wallZ);
            lucidWallObject.transform.localRotation = wallRotation;
            lucidWallObject.layer = LayerMask.NameToLayer(lucidMazeLayerName);

            lucidWallObject.GetComponent<MeshRenderer>().material = lucidWallMaterial;
        }
    }

    private void CreateMaze(Maze maze)
    {
        // Parent GameObject for the lucid maze outline
        lucidMazeOutline = new GameObject
        {
            name = "LucidMazeOutline",
            transform =
            {
                position = transform.localPosition,
                rotation = transform.rotation,
                localScale = transform.localScale
            }
        };

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
                    CreateWall(row, col, direction, Maze.WallType.ExitDoor);
                    continue;
                }

                if (!connectingNodes.Contains(index) && (index < node || isEdge))
                {
                    CreateWall(row, col, direction, Maze.WallType.Wall);
                }
            }
        }

        surface.BuildNavMesh();
    }
}
