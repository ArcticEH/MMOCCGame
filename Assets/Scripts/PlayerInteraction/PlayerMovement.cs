using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] float movementSpeed = 0.5f;

    [SyncVar(hook = nameof(HandleCurrentCellNumberChanged))] [SerializeField] int currentCellNumber;

    public Cell currentCell;

    Cell nextCell;

    public List<Cell> currentPath = new List<Cell>();

    public bool isPathing = false;

    MouseHoveror mouseHoveror;

    // Cached Variables
    Animator playerAnimator;

    private void Awake()
    {
        DepthSorter.DepthSort(FindObjectsOfType<Cell>());
        mouseHoveror = FindObjectOfType<MouseHoveror>();
    }

    public override void OnStartClient()
    {
        if (!isLocalPlayer) { return; }
        //Request to be placed at spawn
        DepthSorter.DepthSort(FindObjectsOfType<Cell>());

        int spawnCellNumber = FindObjectOfType<Spawn>().GetComponent<Cell>().GetCellNumber();
        CmdPlayerMoveToCell(spawnCellNumber);
    }


    [Command]
    public void CmdPlayerMoveToCell(int cellNumber)
    {
        // TODO:  Add some type of verification here
        currentCellNumber = cellNumber;
    }

    private void Update()
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame) { return; }

        if(!isLocalPlayer) { return;  }

        if (mouseHoveror.currentCell == null ) { return;  }

        CmdPlayerMoveToCell(mouseHoveror.currentCell.GetCellNumber());
    }

    public void HandleCurrentCellNumberChanged(int oldCell, int newCell)
    {
        // Set cell transform
        currentCellNumber = newCell;

        // Set player transform to new cell
        Cell cell = FindCellWithNumber(newCell);
        transform.parent = cell.transform;

        // Reset cell transform (probably shouldnt be here but testing for spawn)
        transform.localPosition = Vector3.zero;
    }

    public Cell FindCellWithNumber(int cellNumber)
    {
        Cell[] cells = FindObjectsOfType<Cell>();

        foreach (Cell cell in cells)
        {
            if (cell.GetCellNumber() == cellNumber)
            {
                return cell;
            }
        }

        Debug.LogError("Could not find cell with number - " + cellNumber);
        return null;
    }


    //void Start()
    //{
    //    playerAnimator = GetComponent<Animator>();
    //}

    //public void PerformPathing()
    //{

    //    if (currentPath.Count == 0) { return; }

    //    isPathing = true;
    //    nextCell = currentPath[0];
    //    currentPath.RemoveAt(0);

    //    print($"Next cell - {nextCell.name}");

    //    // Determine which direction this path is in
    //    if (nextCell == currentCell)
    //    {
    //        CompletePath();
    //    }
    //    else
    //    {
    //        StartCoroutine(PerformMovement(nextCell));
    //    }
    //}


    //private IEnumerator PerformMovement(Cell nextCell)
    //{

    //    currentCell = nextCell; // set current cell here since the movement will be completed - otherwise pathfinder may still include previous cell on multiple clicks
    //    var nextCellPosition = nextCell.transform.position;
    //    var midPoint = (transform.position + nextCell.transform.position) / 2f;
    //    var hasHitMidpoint = false;
    //    var midwayPoint = Vector3.Distance(transform.position, nextCellPosition) / 2;

    //    // Animation Triggers
    //    if ((transform.position.x > nextCellPosition.x) && transform.position.y > nextCellPosition.y) // LEFT
    //    {
    //        playerAnimator.SetTrigger("faceLeft");
    //    }
    //    else if ((transform.position.x < nextCellPosition.x) && transform.position.y > nextCellPosition.y) // DOWN
    //    {
    //        playerAnimator.SetTrigger("faceDown");
    //    }
    //    else if ((transform.position.x > nextCellPosition.x) && transform.position.y < nextCellPosition.y) // UP
    //    {
    //        playerAnimator.SetTrigger("faceUp");
    //    }
    //    else if ((transform.position.x < nextCellPosition.x) && transform.position.y < nextCellPosition.y) // RIGHT
    //    {
    //        playerAnimator.SetTrigger("faceRight");
    //    }


    //    while (transform.position != nextCellPosition)
    //    {
    //        // Check if player has past midpoint, once they have then move them to the new cell
    //        if ((Vector3.Distance(transform.position, nextCellPosition) < midwayPoint) && (hasHitMidpoint == false))
    //        {
    //            MoveToCell();
    //            hasHitMidpoint = true;
    //        }

    //        var nextPosition = Vector3.MoveTowards(transform.position, nextCell.transform.position, movementSpeed * Time.deltaTime);
    //        transform.position = nextPosition;
    //        yield return null;
    //    }

    //    MoveToCell(); // for sprite sorting this should happen halfway through
    //    CompletePath();
    //}

    //private void MoveToCell()
    //{
    //    transform.parent = nextCell.transform;
    //    transform.SetSiblingIndex(1);
    //}

    ///// <summary>
    ///// Checks if pathing should continue or if it should stop.
    ///// </summary>
    //private void CompletePath()

    //{
    //    // If there are still cells in pathing, then keep going. Otherwise stop.
    //    if (currentPath.Count != 0)
    //    {
    //        PerformPathing();
    //    }
    //    else
    //    {
    //        isPathing = false;
    //    }

    //}

    //Vector3 TwoDToIso(Vector3 oldCoordinates)
    //{
    //    var newCoordinates = new Vector2();
    //    newCoordinates.x = oldCoordinates.x - oldCoordinates.y;
    //    newCoordinates.y = (oldCoordinates.x + oldCoordinates.y) / 2;
    //    return newCoordinates;
    //}

    //Cell FindCellWithCoordinates(Vector3 coordinates)
    //{
    //    var cells = FindObjectsOfType<Cell>();
    //    foreach (Cell cell in cells)
    //    {
    //        if (cell.transform.position == coordinates)
    //        {
    //            //print("Found the new cell - " + cell.name);
    //            return cell;
    //        }
    //    }

    //    return currentCell;
    //}
}
