using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cell : NetworkBehaviour
{
    [SerializeField] private int sortingLayer;
    [SerializeField] private bool isWalkable = true;
    [SerializeField] private MouseHoveror mouseHoveror;

    // Getters and Setters
    public int GetSortingLayer() {
        return sortingLayer;
    }

    public bool GetIsWalkable()
    {
        return isWalkable;
    }

    #region Server

    public override void OnStartServer()
    {
      
    }



    #endregion


    #region Client

    private void Start()
    {
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
        }
    }

    private void OnMouseEnter()
    {
        Debug.Log("got here");
        // Set current cell and turn on mouse hoveror
        mouseHoveror.gameObject.SetActive(true);
        mouseHoveror.SetCurrentCell(this);

        // Place mouse hoeveror outline in proper spot
        mouseHoveror.transform.parent = transform;
        mouseHoveror.transform.position = transform.position;
        mouseHoveror.transform.SetSiblingIndex(1);
    }


    public Vector2 GetCoordinates()
    {
        return new Vector2(transform.position.x, transform.position.y);
    }

    #endregion
}
