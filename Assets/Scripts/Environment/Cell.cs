﻿using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Cell: MonoBehaviour
{
    [SerializeField] public int cellNumber;
    [SerializeField] public int sortingLayer;
    [SerializeField] private MouseHoveror mouseHoveror;
    [SerializeField] public List<IsometricObject> objectsInCell = new List<IsometricObject>();


    private void Start()
    {
        // Assigned cached vars
        mouseHoveror = FindObjectOfType<MouseHoveror>();
    }

    private void Update()
    {
        // THis is here right now because I cant figure out why the mousehoveror wouldnt exist yet in awake
        if (mouseHoveror == null)
            mouseHoveror = FindObjectOfType<MouseHoveror>();
    }

    public int GetCellNumber()
    {
        return cellNumber;
    }

    public Vector2 GetCoordinates()
    {
        return new Vector2(transform.position.x, transform.position.y);
    }

    public bool GetIsWalkable()
    { 
        // if any objects in cell are not walkable then return false
        foreach (IsometricObject isometricObject in objectsInCell)
        {
            if (!isometricObject.GetIsWalkable())
            {
                return false;
            }
        }

        return true;
    }

    public void AssignObjectOrderLayers()
    {
        // Get all isometric objects on this cell and apply them their sorting orders
        // *each cell is currently reserved 10 sorting layers but can use z values to order objects within the cell as well
        int objectNumber = 0;
        foreach (IsometricObject obj in objectsInCell)
        {
            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            sr.sortingOrder = sortingLayer + objectNumber;
            objectNumber += 1;
        }
    }

    private void OnMouseEnter()
    {
        // Set current cell on mouse hoveror
        mouseHoveror.SetCurrentCell(this);
    }










    // [SerializeField] private int sortingLayer;
    // [SerializeField] private int cellNumber;
    // [SerializeField] private MouseHoveror mouseHoveror;

    // // Getters and Setters
    // public int GetSortingLayer() {
    //     return sortingLayer;
    // }














}
