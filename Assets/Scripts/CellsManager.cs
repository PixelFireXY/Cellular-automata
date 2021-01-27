using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellsManager : MonoBehaviour
{
    private bool[,] currCells;                                  // The cells of the current life cycle.
    private bool[,] newCells;                                   // The cells of the next life cycle.

    private CellBehaviour[,] cellsArray;                        // The cells reference for the objects in the scene.

    [SerializeField]
    private CellBehaviour cellPrefab = null;                    // The prefab of the cell.

    [SerializeField]
    private Transform cellsParent = null;                       // The parent where spawn the cell.

    [SerializeField]
    private Vector2Int cellGridSize = new Vector2Int(10, 10);   // The size of the grid.

    private readonly float cycleRefreshRate = 0.5f;

    private void Awake()
    {
        InitializeArrays();

        InstantiateCells();

        StartCoroutine(CellsLifeCicler());
    }

    /// <summary>
    /// Initialize all arrays for the cells grid.
    /// </summary>
    private void InitializeArrays()
    {
        currCells = new bool[cellGridSize.x, cellGridSize.y];
        newCells = new bool[cellGridSize.x, cellGridSize.y];
        cellsArray = new CellBehaviour[cellGridSize.x, cellGridSize.y];
    }

    /// <summary>
    /// Instantiate the cells and add it to the array.
    /// </summary>
    private void InstantiateCells()
    {
        if (cellPrefab == null)
            return;

        // The start point is top left
        float halfGridSizeX = cellGridSize.x / 2,
            halfGridSizeY = cellGridSize.y / 2;

        float startCellPosX = -halfGridSizeX,
            startCellPosY = halfGridSizeY;

        // Instantiate all cells base on grid size
        for (int y = 0; y < cellGridSize.y; y++)
        {
            for (int x = 0; x < cellGridSize.x; x++)
            {
                float currCellPosX = startCellPosX + x,
                    currCellPosY = startCellPosY - y;

                CellBehaviour cell = Instantiate(cellPrefab, new Vector3(currCellPosX, currCellPosY), Quaternion.identity, cellsParent);

                // Set default color, name and value
                cell.CellColor = Color.black;
                cell.transform.name = $"Cell({x},{y})";
                cellsArray[x, y] = cell;
            }
        }
    }

    /// <summary>
    /// Move to the next life cycle.
    /// </summary>
    private void NextCicle()
    {
        if (newCells == null)
            return;

        // Overwrite the old grid with the new one
        currCells = newCells;

        // Reset the new grid
        newCells = new bool[cellGridSize.x, cellGridSize.y];

        Debug.Log($"New cycle");
    }

    /// <summary>
    /// Check if a cell can live or die.
    /// </summary>
    /// <param name="cellIndex">The index position of the cell in the grid.</param>
    /// <returns>True if the cell can live otherwise false.</returns>
    private bool CheckCellLife(Vector2Int cellIndex)
    {
        // Counter for how many cells we have around us
        var aliveCellsCounter = 0;
        bool isCellAlive = currCells[cellIndex.x, cellIndex.y];

        // We need to count the cells alive around us in a 3x3 area
        for (int y = -1; y < 2; y++)
        {
            for (int x = -1; x < 2; x++)
            {
                int cellToCheckX = cellIndex.x + x,
                    cellToCheckY = cellIndex.y + y;

                // Prevent to go outside of the grid
                if ((cellToCheckX < 0 || cellToCheckY < 0) ||
                    (cellToCheckX >= currCells.GetLength(0) || cellToCheckY >= currCells.GetLength(1)))
                    continue;

                // Prevent to count ourself
                if (x == 0 && y == 0)
                    continue;

                if (currCells[cellToCheckX, cellToCheckY])
                    aliveCellsCounter++;
            }
        }

        if (isCellAlive)
        {
            if (aliveCellsCounter < 2 ||        // Any live cell with fewer than two live neighbours dies, as if by underpopulation.
                aliveCellsCounter > 3)          // Any live cell with more than three live neighbours dies, as if by overpopulation.
            {
                return false;
            }
            else    // Any live cell with two or three live neighbours lives on to the next generation.
            {
                return true;
            }
        }
        else
        {
            // Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.
            if (aliveCellsCounter == 3)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Set a status (alive or dead) at a specific cell.
    /// </summary>
    /// <param name="cellIndex">The index of the cell we want to set the status.</param>
    /// <param name="cellStatus">The status we want to assign to the cell.</param>
    private void SetCellStatus(Vector2Int cellIndex, bool cellStatus)
    {
        cellsArray[cellIndex.x, cellIndex.y].IsCellAlive =
            newCells[cellIndex.x, cellIndex.y] = cellStatus;
    }

    /// <summary>
    /// Used to constantly refresh the cells grid.
    /// </summary>
    private IEnumerator CellsLifeCicler()
    {
        int cellIndexX = currCells.GetLength(0),
            cellIndexY = currCells.GetLength(1);

        while (true)
        {
            yield return new WaitForSeconds(cycleRefreshRate);

            // Refresh every cell in the grid
            for (int y = 0; y < cellIndexY; y++)
            {
                for (int x = 0; x < cellIndexX; x++)
                {
                    var cellToCheck = new Vector2Int(x, y);
                    SetCellStatus(cellToCheck, CheckCellLife(cellToCheck));
                }
            }

            // Go to the next cycle
            NextCicle();
        }
    }
}
