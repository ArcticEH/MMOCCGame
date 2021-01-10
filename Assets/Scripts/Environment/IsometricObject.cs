using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class IsometricObject : MonoBehaviour
{
    [SerializeField] private float height = 0; // Specified by artist when deciding how items should be placed atop
    [SerializeField] private bool isWalkable;
    [SerializeField] public int cellNumber;

    [SerializeField] public Cell myCell;
    [SerializeField] public int cellIndex;

    // Cached
    [SerializeField] public CellsManager Cells;

    #region Getters and Setters

    public float GetHeight()
    {
        return height;
    }

    #endregion


    private void Start()
    {
        // Assign cached variables
        Cells = FindObjectOfType<CellsManager>();

    }

    public void UpdateCell(Cell newCell, int index = -1)
    {
        // Remove from current cell if there is a cell
        if (myCell != null)
            myCell.objectsInCell.Remove(this);

        // Set new cell
        myCell = newCell;

        // Add at specified index
        if (index == -1)
            myCell.objectsInCell.Add(this);
        else
            myCell.objectsInCell.Insert(index, this);
    }

    public void RemoveFromCell()
    {
        if (myCell == null) { return;  }

        myCell.objectsInCell.Remove(this);
    }


    #region Helper

    //public Cell FindMyCell()
    //{
    //    foreach(Cell cell in Cells.cells)
    //    {
    //        if (cell.transform.position == this.transform.position)
    //        {
    //            return cell;
    //        }
    //    }

    //    // Error since we could not find cell
    //    throw new System.Exception("Error finding cell for isometric object - " + gameObject.name);
    //}

    // Determines if player should be able to walk on this cell. Future implementation may consider more factors than just the variable.
    public bool GetIsWalkable()
    {
        return isWalkable;
    }

    #endregion

}
