using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField] private int sortingLayer;
    [SerializeField] private bool isWalkable = true;
    [SerializeField] private int cellNumber;
    [SerializeField] private MouseHoveror mouseHoveror;
    
    // Getters and Setters
    public int GetSortingLayer() {
        return sortingLayer;
    }

    public bool GetIsWalkable()
    {
        return isWalkable;
    }

    public int GetCellNumber()
    {
        return cellNumber;
    }

    
    private void Start()
    {
        // Assigned cached vars
        mouseHoveror = FindObjectOfType<MouseHoveror>();    
    }

    public void AssignObjectOrderLayers(int sortingLayer)
    {
        // Get all isometric objects on this cell and apply them their sorting orders
        // *each cell is currently reserved 10 sorting layers but can use z values to order objects within the cell as well
        var isometricObjects = GetComponentsInChildren<IsometricObject>();
        int objectNumber = 0;
        foreach (IsometricObject obj in isometricObjects)
        {
            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            sr.sortingOrder = sortingLayer + objectNumber;
            objectNumber += 1;
            cellNumber = sortingLayer / 10;
        }
    }

    private void OnMouseEnter()
    {
        // Set current cell on mouse hoveror
        mouseHoveror.SetCurrentCell(this);
    }


    public Vector2 GetCoordinates()
    {
        return new Vector2(transform.position.x, transform.position.y);
    }

  
}
