using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CellsManager : MonoBehaviour
{
    [SerializeField] Cell cellPrefab;
    [SerializeField] public List<Cell> cells;
    [SerializeField] public int sortingLayersPerCell = 10;

    private void Start()
    {
        // Find all cells in scene
        cells = FindObjectsOfType<Cell>().ToList();
        DepthSort();
    }

    private void Update()
    {
        // Depth sort on each frame
        DepthSort();
    }

    #region Helper


    /// <summary>
    /// Uses a selection sort to sort cells and assigns the cells those layers within each sort cycle
    /// </summary>
    /// <param name="cells"></param>
    public void DepthSort()
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
