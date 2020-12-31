using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MouseHoveror : MonoBehaviour
{
    [SerializeField] IsometricObject selectionOutlinePrefab;
    private Cell currentCell;

    public void SetCurrentCell(Cell currentCell)
    {
        this.currentCell = currentCell;
    }

    private void Update()
    {
        if(Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleClick();
        }
    }

    private void HandleClick()
    {
        Debug.Log("Mouse clicked");
    }

}
