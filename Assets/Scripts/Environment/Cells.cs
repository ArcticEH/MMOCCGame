using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cells : MonoBehaviour
{
    [SerializeField] Cell cellPrefab;
    [SerializeField] public List<Cell> cells;
    [SerializeField] public int sortingLayersPerCell = 10;

    private void Start()
    {
        // Find all ground tiles in scene
        GameObject[] groundTiles = GameObject.FindGameObjectsWithTag("Ground");

        // Each ground tile indicates a new cell
        foreach (GameObject groundTile in groundTiles)
        {
            Cell cell = Instantiate(cellPrefab, groundTile.transform.position, Quaternion.identity, this.transform);
            cells.Add(cell);
        }
 
    }

    private void Update()
    {
        DepthSort(cells);
    }

    #region Helper
    /// <summary>
    /// Uses a selection sort to sort cells and assigns the cells those layers within each sort cycle
    /// </summary>
    /// <param name="cells"></param>
    private void DepthSort(List<Cell> cells)
    {
        int n = cells.Count;

        // one by one move boundary of unsorted array
        for (int i = 0; i < n; i++)
        {
            // find min in unsorted array
            int minindex = i;
            for (int j = i + 1; j < n; j++)
            {
                if (cells[j].transform.position.y > cells[minindex].transform.position.y)
                {
                    minindex = j;
                }
                else if ((cells[j].transform.position.y == cells[minindex].transform.position.y) && cells[j].transform.position.x > cells[minindex].transform.position.x)
                {
                    minindex = j;
                }
            }

            // swap found minimum with the first element
            Cell temp = cells[minindex];
            cells[minindex] = cells[i];
            cells[i] = temp;

            // Assign minimum cell its sorting layer
            cells[i].cellNumber = i;
            cells[i].sortingLayer = i * sortingLayersPerCell;
            cells[i].AssignObjectOrderLayers();
            cells[i].name = $"Cell {i}";
        }

        #endregion
    }

}
