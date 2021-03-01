using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsometricObject : MonoBehaviour
{
    [SerializeField] private float height = 0; // Specified by artist when deciding how items should be placed atop
    [SerializeField] public int amountYRaised;
    [SerializeField] private bool isWalkable;
    [SerializeField] public int cellNumber;
    [SerializeField] public Cell myCell;
    [SerializeField] public int cellIndex;

    [SerializeField] public bool assignToCellAutomatically = true; // Used for static objects like furniture

    // Cached
    [SerializeField] public CellsManager cellsManager;

    #region Getters and Setters

    public float GetHeight()
    {
        return height;
    }

    #endregion

      
    private void Start()
    {
        // Assign cached variables
        cellsManager = FindObjectOfType<CellsManager>();

    }

    // Update cell and tell cell to isometric object to attempt to find its own cell
    public void UpdateCell(int amountYRaised, int index = -1)
    {
        this.amountYRaised = amountYRaised;

        // Remove from current cell if there is a cell
        if (myCell != null)
            myCell.objectsInCell.Remove(this);

        // Find new cell
        myCell = FindMyCell(amountYRaised);

        // Add at specified index
        if (index == -1)
            myCell.objectsInCell.Add(this);
        else
            myCell.objectsInCell.Insert(index, this);
    }

    // Update cell and provide isometric object with new cell. Used only for player movement right now because finding cell is an issue when player sprite is between cells
    public void UpdateCell(Cell newCell, int index = -1)
    {
        // Remove from current cell if there is a cell
        if (myCell != null)
            myCell.objectsInCell.Remove(this);

        // Use cell provided
        myCell = newCell;

        // Add at specified index
        if (index == -1)
            myCell.objectsInCell.Add(this);
        else
            myCell.objectsInCell.Insert(index, this);
    }

    // Remove object from cell
    public void RemoveFromCell()
    {
        if (myCell == null) { return;  }

        myCell.objectsInCell.Remove(this);
    }


    #region Helper

    // Find the cell that this isometric object belongs to based on position
    public Cell FindMyCell(int amountYRaised)
    {
        if (name.Equals("smallDoubleBlockLeft"))
        {
            Debug.Log("stop here");
        }

        cellsManager = FindObjectOfType<CellsManager>();

        foreach (Cell cell in cellsManager.cells)
        {
            if (cell.transform.position == new Vector3(transform.position.x, transform.position.y - amountYRaised, transform.position.z))
            {
                return cell;
            }
        }

        // Error since we could not find cell
        throw new System.Exception($"Error finding cell for isometric object - {gameObject.name} , with position - {transform.position} ");
    }

    // Determines if player should be able to walk on this cell. Future implementation may consider more factors than just the variable.
    public bool GetIsWalkable()
    {
        return isWalkable;
    }

    #endregion

}
