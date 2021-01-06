using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class IsometricObject : MonoBehaviour
{
    //[SerializeField] private float height = 0;
    [SerializeField] private bool isWalkable;
    [SerializeField] public int cellNumber;

    [SerializeField] public Cell myCell;

    // Cached
    [SerializeField] public Cells Cells;

    #region Client

    private void Start()
    {
        // Assign cached variables
        Cells = FindObjectOfType<Cells>();

        // Find out what cell this object is on
        myCell = FindMyCell();
        myCell.objectsInCell.Add(this);
    }

    public void UpdateCell(Cell newCell, int index)
    {
        // Remove from current cell 
        myCell.objectsInCell.Remove(this);

        // Set new cell
        myCell = newCell;

        // Add at specified index
        if (index == -1)
            myCell.objectsInCell.Add(this);
        else
            myCell.objectsInCell.Insert(index, this);
    }


    #endregion

    #region Helper

    public Cell FindMyCell()
    {
        foreach(Cell cell in Cells.cells)
        {
            if (cell.transform.position == this.transform.position)
            {
                return cell;
            }
        }

        // Error since we could not find cell
        throw new System.Exception("Error finding cell for isometric object - " + gameObject.name);
    }

    // Determines if player should be able to walk on this cell. Future implementation may consider more factors than just the variable.
    public bool GetIsWalkable()
    {
        return isWalkable;
    }

    #endregion

}
