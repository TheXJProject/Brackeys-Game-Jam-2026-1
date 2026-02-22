using System.Collections.Generic;
using Vector3 = UnityEngine.Vector3;

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
    public float wallOffset;
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

    public Vector3 getOnWallFaceOffset(WallDirection wallFace)
    {
        return wallFace switch
        {
            WallDirection.North => (Vector3.forward * scale.z / 2) + (Vector3.up * scale.y / 2),
            WallDirection.South => (Vector3.back * scale.z / 2) + (Vector3.up * scale.y / 2),
            WallDirection.East => (Vector3.right * scale.x / 2) + (Vector3.up * scale.y / 2),
            WallDirection.West => (Vector3.left * scale.x / 2) + (Vector3.up * scale.y / 2),
            _ => Vector3.zero
        };
    }

    public Vector3 getOnWallFaceRotation(WallDirection wallFace)
    {
        return wallFace switch
        {
            WallDirection.North => Vector3.up * -180,
            WallDirection.South => Vector3.zero,
            WallDirection.East => Vector3.up * -90,
            WallDirection.West => Vector3.up * -270,
            _ => Vector3.zero
        };
    }
}


public static class Mazes
{
    public static readonly Maze LevelOneMaze = new(
        nodeConnections: new Dictionary<int, HashSet<int>>
        {
            { 0, new HashSet<int>() },
            { 1, new HashSet<int>() },
            { 2, new HashSet<int> { 3, 11 } },
            { 3, new HashSet<int> { 2 } },
            { 4, new HashSet<int> { 13 } },
            { 5, new HashSet<int> { 6 } },
            { 6, new HashSet<int> { 15 } },
            { 7, new HashSet<int>() },
            { 8, new HashSet<int>() },
            { 9, new HashSet<int>() },
            { 10, new HashSet<int>() },
            { 11, new HashSet<int> { 2, 12, 20 } },
            { 12, new HashSet<int> { 11, 13, 21 } },
            { 13, new HashSet<int> { 4, 12, 14 } },
            { 14, new HashSet<int> { 13, 23 } },
            { 15, new HashSet<int> { 6, 24 } },
            { 16, new HashSet<int>() },
            { 17, new HashSet<int>() },
            { 18, new HashSet<int>() },
            { 19, new HashSet<int>() },
            { 20, new HashSet<int> { 11, 29 } },
            { 21, new HashSet<int> { 12, 22 } },
            { 22, new HashSet<int> { 21 } },
            { 23, new HashSet<int> { 14, 24, 32 } },
            { 24, new HashSet<int> { 15, 23, 33 } },
            { 25, new HashSet<int>() },
            { 26, new HashSet<int>() },
            { 27, new HashSet<int>() },
            { 28, new HashSet<int>() },
            { 29, new HashSet<int> { 20, 30 } },
            { 30, new HashSet<int> { 29 } },
            { 31, new HashSet<int> { 32, 40 } },
            { 32, new HashSet<int> { 23, 31 } },
            { 33, new HashSet<int> { 24 } },
            { 34, new HashSet<int>() },
            { 35, new HashSet<int>() },
            { 36, new HashSet<int>() },
            { 37, new HashSet<int>() },
            { 38, new HashSet<int>() },
            { 39, new HashSet<int>() },
            { 40, new HashSet<int> { 31, 49 } },
            { 41, new HashSet<int>() },
            { 42, new HashSet<int>() },
            { 43, new HashSet<int>() },
            { 44, new HashSet<int>() },
            { 45, new HashSet<int>() },
            { 46, new HashSet<int>() },
            { 47, new HashSet<int>() },
            { 48, new HashSet<int>() },
            { 49, new HashSet<int> { 40, 58 } },
            { 50, new HashSet<int>() },
            { 51, new HashSet<int>() },
            { 52, new HashSet<int>() },
            { 53, new HashSet<int>() },
            { 54, new HashSet<int>() },
            { 55, new HashSet<int>() },
            { 56, new HashSet<int>() },
            { 57, new HashSet<int>() },
            { 58, new HashSet<int> { 49, 67 } },
            { 59, new HashSet<int>() },
            { 60, new HashSet<int>() },
            { 61, new HashSet<int>() },
            { 62, new HashSet<int>() },
            { 63, new HashSet<int>() },
            { 64, new HashSet<int>() },
            { 65, new HashSet<int>() },
            { 66, new HashSet<int>() },
            { 67, new HashSet<int> { 58, 76 } },
            { 68, new HashSet<int>() },
            { 69, new HashSet<int>() },
            { 70, new HashSet<int>() },
            { 71, new HashSet<int>() },
            { 72, new HashSet<int>() },
            { 73, new HashSet<int>() },
            { 74, new HashSet<int>() },
            { 75, new HashSet<int>() },
            { 76, new HashSet<int> { 67 } },
            { 77, new HashSet<int>() },
            { 78, new HashSet<int>() },
            { 79, new HashSet<int>() },
            { 80, new HashSet<int>() },
        },
        size: 9,
        startNode: (76, Maze.WallDirection.South),
        endNode: (3, Maze.WallDirection.North)
    );

    public static readonly Maze LevelTwoMaze = new(
        nodeConnections: new Dictionary<int, HashSet<int>>
        {
            { 0, new HashSet<int>() },
            { 1, new HashSet<int>() },
            { 2, new HashSet<int> { 3, 10 } },
            { 3, new HashSet<int> { 2, 4 } },
            { 4, new HashSet<int> { 3, 5, 12 } },
            { 5, new HashSet<int> { 4, 6 } },
            { 6, new HashSet<int> { 5, 14 } },
            { 7, new HashSet<int>() },
            { 8, new HashSet<int>() },
            { 9, new HashSet<int>() },
            { 10, new HashSet<int> { 2, 11 } },
            { 11, new HashSet<int> { 10 } },
            { 12, new HashSet<int> { 4, 20 } },
            { 13, new HashSet<int>() },
            { 14, new HashSet<int> { 6, 22 } },
            { 15, new HashSet<int>() },
            { 16, new HashSet<int>() },
            { 17, new HashSet<int>() },
            { 18, new HashSet<int> { 26 } },
            { 19, new HashSet<int> { 20, 27 } },
            { 20, new HashSet<int> { 12, 19 } },
            { 21, new HashSet<int> { 22, 29 } },
            { 22, new HashSet<int> { 14, 21, 30 } },
            { 23, new HashSet<int>() },
            { 24, new HashSet<int>() },
            { 25, new HashSet<int>() },
            { 26, new HashSet<int> { 18, 27, 34 } },
            { 27, new HashSet<int> { 19, 26 } },
            { 28, new HashSet<int> { 29, 36 } },
            { 29, new HashSet<int> { 21, 28, 37 } },
            { 30, new HashSet<int> { 22, 38 } },
            { 31, new HashSet<int>() },
            { 32, new HashSet<int>() },
            { 33, new HashSet<int>() },
            { 34, new HashSet<int> { 42, 26 } },
            { 35, new HashSet<int> { 36, 43 } },
            { 36, new HashSet<int> { 28, 35 } },
            { 37, new HashSet<int> { 29, 45 } },
            { 38, new HashSet<int> { 30 } },
            { 39, new HashSet<int>() },
            { 40, new HashSet<int>() },
            { 41, new HashSet<int>() },
            { 42, new HashSet<int> { 34, 43 } },
            { 43, new HashSet<int> { 35, 42, 44 } },
            { 44, new HashSet<int> { 43, 45 } },
            { 45, new HashSet<int> { 37, 44, 46, 53 } },
            { 46, new HashSet<int> { 45, 54 } },
            { 47, new HashSet<int>() },
            { 48, new HashSet<int>() },
            { 49, new HashSet<int>() },
            { 50, new HashSet<int> { 51 } },
            { 51, new HashSet<int> { 50, 52 } },
            { 52, new HashSet<int> { 51, 60 } },
            { 53, new HashSet<int> { 45, 61 } },
            { 54, new HashSet<int> { 46, 62 } },
            { 55, new HashSet<int>() },
            { 56, new HashSet<int>() },
            { 57, new HashSet<int>() },
            { 58, new HashSet<int> { 59 } },
            { 59, new HashSet<int> { 58, 60 } },
            { 60, new HashSet<int> { 52, 59, 61 } },
            { 61, new HashSet<int> { 53, 60, 62 } },
            { 62, new HashSet<int> { 54, 61 } },
            { 63, new HashSet<int>() },
        },
        size: 8,
        startNode: (10, Maze.WallDirection.West),
        endNode: (18, Maze.WallDirection.West)
    );

    public static readonly Maze LevelThreeMaze = new(
        nodeConnections: new Dictionary<int, HashSet<int>>
        {
            { 0, new HashSet<int> { 8 } },
            { 1, new HashSet<int> { 2 } },
            { 2, new HashSet<int> { 1, 3, 10 } },
            { 3, new HashSet<int> { 2, 4 } },
            { 4, new HashSet<int> { 3, 5 } },
            { 5, new HashSet<int> { 4, 6 } },
            { 6, new HashSet<int> { 5, 7, 14 } },
            { 7, new HashSet<int> { 6, 15 } },
            { 8, new HashSet<int> { 0, 9, 16 } },
            { 9, new HashSet<int> { 8, 10, 17 } },
            { 10, new HashSet<int> { 2, 9 } },
            { 11, new HashSet<int> { 12, 19 } },
            { 12, new HashSet<int> { 11, 13, 20 } },
            { 13, new HashSet<int> { 12, 14, 21 } },
            { 14, new HashSet<int> { 13, 22, 6 } },
            { 15, new HashSet<int> { 7, 23 } },
            { 16, new HashSet<int> { 8, 24 } },
            { 17, new HashSet<int> { 9, 25, 18 } },
            { 18, new HashSet<int> { 17, 19, 26 } },
            { 19, new HashSet<int> { 18, 11, 20 } },
            { 20, new HashSet<int> { 19, 12, 21, 28 } },
            { 21, new HashSet<int> { 20, 13, 22 } },
            { 22, new HashSet<int> { 21, 14, 30, 23 } },
            { 23, new HashSet<int> { 15, 22 } },
            { 24, new HashSet<int> { 16 } },
            { 25, new HashSet<int> { 17, 33, 26 } },
            { 26, new HashSet<int> { 18, 25, 34 } },
            { 27, new HashSet<int> { 35 } },
            { 28, new HashSet<int> { 20, 36, 29 } },
            { 29, new HashSet<int> { 37, 28 } },
            { 30, new HashSet<int> { 22, 31 } },
            { 31, new HashSet<int> { 30, 39 } },
            { 32, new HashSet<int> { 40, 33 } },
            { 33, new HashSet<int> { 32, 25 } },
            { 34, new HashSet<int> { 26, 42 } },
            { 35, new HashSet<int> { 27, 43, 36 } },
            { 36, new HashSet<int> { 35, 28, 44 } },
            { 37, new HashSet<int> { 29, 45, 38 } },
            { 38, new HashSet<int> { 37 } },
            { 39, new HashSet<int> { 31, 47 } },
            { 40, new HashSet<int> { 32, 48 } },
            { 41, new HashSet<int> { 49 } },
            { 42, new HashSet<int> { 34, 50 } },
            { 43, new HashSet<int> { 35 } },
            { 44, new HashSet<int> { 36 } },
            { 45, new HashSet<int> { 37, 46 } },
            { 46, new HashSet<int> { 45, 47 } },
            { 47, new HashSet<int> { 46, 39, 55 } },
            { 48, new HashSet<int> { 40, 49 } },
            { 49, new HashSet<int> { 48, 41, 50 } },
            { 50, new HashSet<int> { 42, 51, 49 } },
            { 51, new HashSet<int> { 50, 52, 59 } },
            { 52, new HashSet<int> { 51, 53 } },
            { 53, new HashSet<int> { 52, 54 } },
            { 54, new HashSet<int> { 53 } },
            { 55, new HashSet<int> { 47, 63 } },
            { 56, new HashSet<int> { 57 } },
            { 57, new HashSet<int> { 56, 58 } },
            { 58, new HashSet<int> { 57, 59 } },
            { 59, new HashSet<int> { 58, 60, 51 } },
            { 60, new HashSet<int> { 61, 59 } },
            { 61, new HashSet<int> { 60, 62 } },
            { 62, new HashSet<int> { 61, 63 } },
            { 63, new HashSet<int> { 62, 55 } },
        },
        size: 8,
        startNode: (63, Maze.WallDirection.East),
        endNode: (56, Maze.WallDirection.West)
    );

    public static readonly Maze LevelFourMaze = new(
        nodeConnections: new Dictionary<int, HashSet<int>>
        {
            { 0, new HashSet<int> { 1, 10 } },
            { 1, new HashSet<int> { 0, 2, 11 } },
            { 2, new HashSet<int> { 1, 3, 12 } },
            { 3, new HashSet<int> { 2, 4 } },
            { 4, new HashSet<int> { 3, 14 } },
            { 5, new HashSet<int> { 15, 6 } },
            { 6, new HashSet<int> { 5, 7 } },
            { 7, new HashSet<int> { 6, 8, 17 } },
            { 8, new HashSet<int> { 7, 9 } },
            { 9, new HashSet<int> { 8, 19 } },
            { 10, new HashSet<int> { 0, 20 } },
            { 11, new HashSet<int> { 1, 12 } },
            { 12, new HashSet<int> { 11, 2, 22 } },
            { 13, new HashSet<int> { 23 } },
            { 14, new HashSet<int> { 4, 15, 24 } },
            { 15, new HashSet<int> { 14, 5, 16 } },
            { 16, new HashSet<int> { 15, 26, 17 } },
            { 17, new HashSet<int> { 7, 16, 27 } },
            { 18, new HashSet<int> { 19 } },
            { 19, new HashSet<int> { 18, 9 } },
            { 20, new HashSet<int> { 10, 30 } },
            { 21, new HashSet<int> { 31, 32 } },
            { 22, new HashSet<int> { 12, 21, 23, 32 } },
            { 23, new HashSet<int> { 22, 13, 24, 33 } },
            { 24, new HashSet<int> { 23, 14, 25, 34 } },
            { 25, new HashSet<int> { 24, 35 } },
            { 26, new HashSet<int> { 16, 27, 36 } },
            { 27, new HashSet<int> { 26, 17, 28 } },
            { 28, new HashSet<int> { 27, 29, 38 } },
            { 29, new HashSet<int> { 28, 39 } },
            { 30, new HashSet<int> { 20, 31, 40 } },
            { 31, new HashSet<int> { 21, 30, 41 } },
            { 32, new HashSet<int> { 22, 42 } },
            { 33, new HashSet<int> { 23, 43 } },
            { 34, new HashSet<int> { 24, 44 } },
            { 35, new HashSet<int> { 25, 45 } },
            { 36, new HashSet<int> { 26, 46 } },
            { 37, new HashSet<int>() },
            { 38, new HashSet<int> { 28, 39, 48 } },
            { 39, new HashSet<int> { 38, 29, 49 } },
            { 40, new HashSet<int> { 30, 50 } },
            { 41, new HashSet<int> { 31, 42, 51 } },
            { 42, new HashSet<int> { 41, 32, 43 } },
            { 43, new HashSet<int> { 42, 33, 53 } },
            { 44, new HashSet<int> { 34, 45 } },
            { 45, new HashSet<int> { 44, 35, 46, 55 } },
            { 46, new HashSet<int> { 45, 36, 47, 56 } },
            { 47, new HashSet<int> { 46, 48, 57 } },
            { 48, new HashSet<int> { 47, 38, 58 } },
            { 49, new HashSet<int> { 39, 59 } },
            { 50, new HashSet<int> { 40, 60 } },
            { 51, new HashSet<int> { 41, 52, 61 } },
            { 52, new HashSet<int> { 51, 53, 62 } },
            { 53, new HashSet<int> { 52, 43, 54 } },
            { 54, new HashSet<int> { 53 } },
            { 55, new HashSet<int> { 45, 65 } },
            { 56, new HashSet<int> { 46, 66 } },
            { 57, new HashSet<int> { 47, 67 } },
            { 58, new HashSet<int> { 48, 68 } },
            { 59, new HashSet<int> { 49, 69 } },
            { 60, new HashSet<int> { 50, 70 } },
            { 61, new HashSet<int> { 51, 71, 62 } },
            { 62, new HashSet<int> { 52, 61, 72 } },
            { 63, new HashSet<int> { 73, 64 } },
            { 64, new HashSet<int> { 63, 65, 74 } },
            { 65, new HashSet<int> { 55, 64, 66, 75 } },
            { 66, new HashSet<int> { 56, 65, 76 } },
            { 67, new HashSet<int> { 57, 77 } },
            { 68, new HashSet<int> { 58 } },
            { 69, new HashSet<int> { 59, 79 } },
            { 70, new HashSet<int> { 60, 80 } },
            { 71, new HashSet<int> { 61, 81 } },
            { 72, new HashSet<int> { 62, 73 } },
            { 73, new HashSet<int> { 72, 63, 83 } },
            { 74, new HashSet<int> { 64, 84 } },
            { 75, new HashSet<int> { 65, 76, 85 } },
            { 76, new HashSet<int> { 75, 66 } },
            { 77, new HashSet<int> { 67, 78, 87 } },
            { 78, new HashSet<int> { 77, 88 } },
            { 79, new HashSet<int> { 69, 89 } },
            { 80, new HashSet<int> { 70, 81, 90 } },
            { 81, new HashSet<int> { 71, 80, 82, 91 } },
            { 82, new HashSet<int> { 81, 92, 83 } },
            { 83, new HashSet<int> { 82, 73 } },
            { 84, new HashSet<int> { 74, 85, 94 } },
            { 85, new HashSet<int> { 75, 84, 86 } },
            { 86, new HashSet<int> { 85, 87 } },
            { 87, new HashSet<int> { 88, 86, 77, 97 } },
            { 88, new HashSet<int> { 78, 87, 98 } },
            { 89, new HashSet<int> { 79, 99 } },
            { 90, new HashSet<int> { 80 } },
            { 91, new HashSet<int> { 81, 92 } },
            { 92, new HashSet<int> { 91, 82, 93 } },
            { 93, new HashSet<int> { 92, 94 } },
            { 94, new HashSet<int> { 93, 84, 95 } },
            { 95, new HashSet<int> { 94, 96 } },
            { 96, new HashSet<int> { 95, 97 } },
            { 97, new HashSet<int> { 96, 87, 98 } },
            { 98, new HashSet<int> { 97, 88 } },
            { 99, new HashSet<int> { 89 } },
        },
        size: 10,
        startNode: (99, Maze.WallDirection.South),
        endNode: (90, Maze.WallDirection.South)
    );

    public static Maze[] allMazes = { LevelOneMaze, LevelTwoMaze, LevelThreeMaze, LevelFourMaze };
}
