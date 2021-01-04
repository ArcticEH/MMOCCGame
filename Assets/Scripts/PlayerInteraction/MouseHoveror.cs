using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseHoveror : MonoBehaviour
{
    [SerializeField] public Cell currentCell;
    [SerializeField] private IsometricObject selectedCellSprite;

    private void Awake()
    {
        selectedCellSprite = FindObjectOfType<CurrentCellSprite>().GetComponent<IsometricObject>();
    }

    public void SetCurrentCell(Cell cell)
    {
        currentCell = cell;

        // Place current sprite outline in cell
        selectedCellSprite.ChangeCell(currentCell, 1);
        selectedCellSprite.transform.position = selectedCellSprite.myCell.transform.position;
    }
}
