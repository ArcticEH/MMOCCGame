using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseHoveror : MonoBehaviour
{
    [SerializeField] public Cell currentCell;
    [SerializeField] private CurrentCellSprite selectedCellSprite;

    private void Start()
    {
        selectedCellSprite = FindObjectOfType<CurrentCellSprite>();
    }

    public void SetCurrentCell(Cell cell)
    {
        currentCell = cell;

        // Place current sprite outline in cell
        selectedCellSprite.transform.parent = cell.transform;
        selectedCellSprite.transform.position = cell.transform.position;
        selectedCellSprite.transform.SetSiblingIndex(1);
    }
}
