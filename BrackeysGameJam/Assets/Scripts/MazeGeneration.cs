using System;
using System.Collections.Generic;
using UnityEngine;

public class MazeGeneration : MonoBehaviour
{
    public GameObject wallGameObject;
    public int cellWidth;

    private float wallOffset;
    private Quaternion aboveBelowRotation = Quaternion.Euler(90, 0, 0);
    private Quaternion leftRightRotation = Quaternion.Euler(0, 90, 0);

    private enum WallDirection
    {
        Left,
        Right,
        Above,
        Below
    }

    // A Maze is represented as a dictionary where:
    // Key - Node index
    // Value - Connecting nodes

    // TODO: Load maze from file
    private Dictionary<int, HashSet<int>> maze = new()
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
    };

    private void CreateWall(int row, int col, WallDirection direction)
    {
        float wallX = col * cellWidth;
        float wallY = -row * cellWidth;
        Quaternion wallRotation = new Quaternion();

        switch (direction)
        {
            case WallDirection.Above:
                wallY += wallOffset;
                wallRotation = aboveBelowRotation;
                break;
            case WallDirection.Below:
                wallY -= wallOffset;
                wallRotation = aboveBelowRotation;
                break;
            case WallDirection.Left:
                wallX -= wallOffset;
                wallRotation = leftRightRotation;
                break;
            case WallDirection.Right:
                wallX += wallOffset;
                wallRotation = leftRightRotation;
                break;
        }

        GameObject wallInstance = Instantiate(wallGameObject, transform);

        wallInstance.transform.localPosition = new Vector3(wallX, wallY, 0);
        wallInstance.transform.localRotation = wallRotation;
        wallInstance.transform.localScale = new Vector3(cellWidth, cellWidth, wallInstance.transform.localScale.z);
    }

    private void CreateWalls()
    {
        const int mazeSize = 10; // The width/height of the maze

        foreach (var node in maze)
        {
            // Key is the node index
            // Value is the connected nodes

            int row = node.Key / mazeSize;
            int col = node.Key % mazeSize;

            int above = node.Key - mazeSize;
            int below = node.Key + mazeSize;
            int left = node.Key - 1;
            int right = node.Key + 1;

            if (!node.Value.Contains(above))
            {
                CreateWall(row, col, WallDirection.Above);
            }

            if (!node.Value.Contains(below))
            {
                CreateWall(row, col, WallDirection.Below);
            }

            if (!node.Value.Contains(left))
            {
                CreateWall(row, col, WallDirection.Left);
            }

            if (!node.Value.Contains(right))
            {
                CreateWall(row, col, WallDirection.Right);
            }
        }
    }

    void Awake()
    {
        wallOffset = cellWidth / 2.0f;
        CreateWalls();
    }
}