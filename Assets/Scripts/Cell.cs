using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Cell : NetworkBehaviour
{
    List<uint> objects = new List<uint>();
    [SerializeField] private int sortingLayer;

    // Getters and Setters
    public int GetSortingLayer() {
        return sortingLayer;
    }

    #region Server

    public override void OnStartServer()
    {
        GetCurrentChildObjects();
    }

    // Gets all child isometric objects in scene
    private void GetCurrentChildObjects()
    {
        var isometricObjects = GetComponentsInChildren<IsometricObject>();
        foreach (IsometricObject isometricObject in isometricObjects)
        {
            objects.Add(isometricObject.GetComponent<NetworkIdentity>().netId);
        }
    }

    #endregion


    #region Client

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

    public Vector2 GetCoordinates()
    {
        return new Vector2(transform.position.x, transform.position.y);
    }

    #endregion
}
