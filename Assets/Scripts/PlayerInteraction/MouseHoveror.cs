using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseHoveror : MonoBehaviour
{
    [SerializeField] public Cell currentCell;
    [SerializeField] private CellObject selectedCellSprite;
    [SerializeField] LayerMask cellLayer;

    private void Update()
    {
        CurrentCellRaycast();
    }

    private void CurrentCellRaycast()
    {
        // Create ray on every frame from current mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 10f, cellLayer);

        // If hit - only change cell if its not the same as current
        if (hit)
        {
            Cell hitCell = hit.collider.gameObject.GetComponent<Cell>();

            if (hitCell == currentCell) { return; }
            SetCurrentCell(hitCell);
            return;
        }

        // Otherwise, set no cell is one is selected
        if (currentCell != null)
        {
            RemoveFromCell();
        }
    }

    private void PlayerLabelRaycast()
    {


       
    }

    public void SetCurrentCell(Cell cell)
    {
        currentCell = cell;

        // Place current sprite outline in cell
        selectedCellSprite.UpdateCell(cell, 0);
        selectedCellSprite.FixPositionToCell();
        selectedCellSprite.gameObject.SetActive(true);
    }

    public void RemoveFromCell()
    {
        currentCell = null;
        selectedCellSprite.RemoveFromCell();
        selectedCellSprite.gameObject.SetActive(false);
        
    }

}
