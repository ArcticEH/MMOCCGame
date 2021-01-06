using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseHoveror : MonoBehaviour
{
    [SerializeField] public Cell currentCell;
    [SerializeField] private CellObject selectedCellSprite;

    private void Awake()
    {
        selectedCellSprite = FindObjectOfType<CurrentCellSprite>().GetComponent<CellObject>();
    }

    public void SetCurrentCell(Cell cell)
    {
        currentCell = cell;

        // Place current sprite outline in cell
        selectedCellSprite.UpdateCell(cell, 1);
        selectedCellSprite.FixPositionToCell();
    }
}
