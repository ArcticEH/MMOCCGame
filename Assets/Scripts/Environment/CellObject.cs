using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellObject : MonoBehaviour
{
    [SerializeField] IsometricObject[] myIsometricObjects;
    [SerializeField] Cell myCell;
    [SerializeField] private int amountYRaised;

    private void Start()
    {
        myIsometricObjects = GetComponentsInChildren<IsometricObject>();

        // Assign cell objects

            foreach (IsometricObject io in myIsometricObjects)
            {
                io.UpdateCell(amountYRaised);
            }
        

    }

    //// Update cell and tell each isometric to find its cell by using the amountYRaised value
    //public void UpdateCell(int index = -1)
    //{
    //    myCell = null; // TODO: Pick cell based off what cell was actually clicked (base cell for a multi-celled object)
    //    // Update cells for all children
    //    foreach (IsometricObject io in myIsometricObjects)
    //    {
    //        io.UpdateCell(amountYRaised, index);
    //    }
    //}

    // Update cell and explicitly provide new cell to move to
    public void UpdateCell(Cell newCell, int index = -1)
    {
        myCell = newCell;
        // Update cells for all children
        foreach (IsometricObject io in myIsometricObjects)
        {
            io.UpdateCell(newCell, index);
        }
    }

    public void RemoveFromCell()
    {
        foreach (IsometricObject io in myIsometricObjects)
        {
            io.RemoveFromCell();
        }
    }


    public void FixPositionToCell()
    {
        transform.position = myCell.transform.position;
    }
}
