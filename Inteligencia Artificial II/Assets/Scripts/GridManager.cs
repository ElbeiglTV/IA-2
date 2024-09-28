using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject cellPrefab;
    public int gridWidth = 10;
    public int gridHeight = 5;
    public float cellSize = 1f;

    private void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 position = new Vector3(x * cellSize, 0, y * cellSize);
                GameObject newCell = Instantiate(cellPrefab, position, Quaternion.identity, transform);
                newCell.GetComponent<Cell>().position = position;
            }
        }
    }
}

