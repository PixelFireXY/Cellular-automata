using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellsManager : MonoBehaviour
{
    public static Action OnCycleIsEnded;

    private bool[,] currCells;
    private bool[,] newCells;

    private CellBehaviour[,] cellsArray;

    [SerializeField]
    private CellBehaviour cellPrefab = null;

    [SerializeField]
    private Transform cellsParent = null;

    [SerializeField]
    [Range(0, 1080)]
    private int gridSizeX = 10,
        gridSizeY = 10;

    private void Awake()
    {
        InitializeArray();

        InstantiateCells();

        StartCoroutine(CellsLifeCicler());
    }

    private void InitializeArray()
    {
        currCells = new bool[gridSizeX, gridSizeY];
        newCells = new bool[gridSizeX, gridSizeY];
        cellsArray = new CellBehaviour[gridSizeX, gridSizeY];
    }

    private void InstantiateCells()
    {
        if (cellPrefab == null)
            return;

        float halfGridSizeX = gridSizeX / 2,
            halfGridSizeY = gridSizeY / 2;

        // The start point is top left
        float startCellPosX = -halfGridSizeX,
            startCellPosY = halfGridSizeY;

        for (int y = 0; y < gridSizeY; y++)
        {
            for (int x = 0; x < gridSizeX; x++)
            {
                float currCellPosX = startCellPosX + x,
                    currCellPosY = startCellPosY - y;

                CellBehaviour cell = Instantiate(cellPrefab, new Vector3(currCellPosX, currCellPosY), Quaternion.identity, cellsParent);

                if (x == 4 && y == 5 ||
                    x == 5 && y == 5 ||
                    x == 6 && y == 5 ||
                    x == 5 && y == 6 ||
                    x == 4 && y == 6 ||
                    x == 3 && y == 6)
                {
                    cell.CellColor = Color.white;

                    currCells[x, y] = true;
                }
                else
                    cell.CellColor = Color.black;

                cell.transform.name = $"Cell({x},{y})";

                cellsArray[x, y] = cell;
            }
        }
    }

    private void NextCicle()
    {
        if (newCells == null)
            return;

        currCells = newCells;

        newCells = new bool[gridSizeX, gridSizeY];

        Debug.Log($"New cycle");
    }

    private bool CheckCellLife(Vector2Int cellIndex)
    {
        var aliveCellsCounter = 0;
        bool isCellAlive = currCells[cellIndex.x, cellIndex.y];

        for (int y = -1; y < 2; y++)
        {
            for (int x = -1; x < 2; x++)
            {
                int cellToCheckX = cellIndex.x + x,
                    cellToCheckY = cellIndex.y + y;

                if ((cellToCheckX < 0 || cellToCheckY < 0) ||
                    (cellToCheckX >= currCells.GetLength(0) || cellToCheckY >= currCells.GetLength(1)) ||
                    (x == 0 && y == 0))
                    continue;

                if (currCells[cellToCheckX, cellToCheckY])
                    aliveCellsCounter++;
            }
        }

        if (isCellAlive)
        {
            // Any live cell with fewer than two live neighbours dies, as if by underpopulation.
            if (aliveCellsCounter < 2)
            {
                // Cell is dead!
                return false;
            }
            // Any live cell with two or three live neighbours lives on to the next generation.
            else if (aliveCellsCounter == 2 || aliveCellsCounter == 3)
            {
                // Cell is alive!
                return true;
            }
            // Any live cell with more than three live neighbours dies, as if by overpopulation.
            else if (aliveCellsCounter > 3)
            {
                // Cell is dead!
                return false;
            }
        }
        else
        {
            // Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.
            if (aliveCellsCounter == 3)
            {
                // Cell is alive!
                return true;
            }
        }

        return false;
    }

    private void SetCellStatus(Vector2Int cellIndex, bool cellStatus)
    {
        cellsArray[cellIndex.x, cellIndex.y].IsCellAlive =
            newCells[cellIndex.x, cellIndex.y] = cellStatus;
    }

    private IEnumerator CellsLifeCicler()
    {
        int cellIndexX = currCells.GetLength(0),
            cellIndexY = currCells.GetLength(1);

        while (true)
        {
            yield return new WaitForSeconds(.5f);

            for (int y = 0; y < cellIndexY; y++)
            {
                for (int x = 0; x < cellIndexX; x++)
                {
                    var cellToCheck = new Vector2Int(x, y);
                    SetCellStatus(cellToCheck, CheckCellLife(cellToCheck));
                }
            }

            NextCicle();
        }
    }
}
