using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellObject : MonoBehaviour
{
    [SerializeField] IsometricObject[] myIsometricObjects;
    [SerializeField] Cell myCell;

    private void Start()
    {
        myIsometricObjects = GetComponentsInChildren<IsometricObject>();
    }

    public void UpdateCell(Cell newCell, int index = -1)
    {
        myCell = newCell;
        // Update cells for all children
        foreach (IsometricObject io in myIsometricObjects)
        {
            io.UpdateCell(newCell, index);
        }
    }

    public void FixPositionToCell()
    {
        transform.position = myCell.transform.position;
    }
}
