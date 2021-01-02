using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    float movementSpeed = 50f;

    [SyncVar(hook = nameof(HandleDestinationCellChanged))] int destinationCell;
    [SyncVar(hook = nameof(HandleCurrentCellChanged))] int currentCellNumber;


    PathFinder pathfinder;

    public Cell currentCell;

    Cell nextCell;

    public List<Cell> currentPath;

    public bool isPathing = false;

    MouseHoveror mouseHoveror;

    // Cached Variables
    Animator playerAnimator;

    private void Awake()
    {
        DepthSorter.DepthSort(FindObjectsOfType<Cell>());
        mouseHoveror = FindObjectOfType<MouseHoveror>();
        pathfinder = FindObjectOfType<PathFinder>();
        playerAnimator = GetComponent<Animator>();
        destinationCell = FindObjectOfType<Spawn>().GetComponent<Cell>().GetCellNumber();
    }

    private void Start()
    {
        // Place where server says parent cell is
        transform.parent = FindCellWithNumber(currentCellNumber).transform;
        transform.SetSiblingIndex(1);
        transform.localPosition = Vector3.zero;
        currentCell = FindCellWithNumber(currentCellNumber);

        // Start pathing if player is currently moving
        if (currentCellNumber != destinationCell)
        {
            pathfinder.SetStartingCells(currentCell, FindCellWithNumber(destinationCell));
            currentPath = pathfinder.GetPath();
            if (isPathing) { return; }
            PerformPathing();
        }

    }

    public override void OnStartServer()
    {
        currentCellNumber = FindObjectOfType<Spawn>().GetComponent<Cell>().GetCellNumber(); 
    }


    [Command]
    public void CmdSetDestinationCell(int cellNumber)
    {
        // TODO:  Add some type of verification here
        destinationCell = cellNumber;
    }

    [Command]
    public void CmdSetCurrentCell(int cellNumber)
    {
        // Intended use right now is to sync this so the server has an idea of where the player is at 
        currentCellNumber = cellNumber;
        
    }

    private void Update()
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame) { return; }

        if(!isLocalPlayer) { return;  }

        if (mouseHoveror.currentCell == null ) { return;  }

        CmdSetDestinationCell(mouseHoveror.currentCell.GetCellNumber());
        CmdSetCurrentCell(mouseHoveror.currentCell.GetCellNumber());
    }

    public void HandleDestinationCellChanged(int oldCell, int newCell)
    {
        Debug.Log("Handle Destiniation Cell Changed");

        // For some reason, even though this is a hook - if I dont run that code as local player then it will have issues deserializing. This makes no sense since this is client side code anyways
        // But we have to set it anyways for now...
        if (isClient)
        {
            pathfinder.SetStartingCells(currentCell, FindCellWithNumber(newCell));
            currentPath = pathfinder.GetPath();
            if (isPathing) { return; }
            PerformPathing();
        }
    }

    public void HandleCurrentCellChanged(int oldcell, int newCell)
    {
        Debug.Log("Old current cell - " + oldcell);
        Debug.Log("Current cell changed to - " + newCell);
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


    public void PerformPathing()
    {

        if (currentPath.Count == 0) { return; }

        isPathing = true;
        nextCell = currentPath[0];
        currentPath.RemoveAt(0);

        print($"Next cell - {nextCell.name}");

        // Determine which direction this path is in
        if (nextCell == currentCell)
        {
            CompletePath();
        }
        else
        {
            StartCoroutine(PerformMovement(nextCell));
        }
    }


    private IEnumerator PerformMovement(Cell nextCell)
    {

        currentCell = nextCell; // set current cell here since the movement will be completed - otherwise pathfinder may still include previous cell on multiple clicks

        if (isLocalPlayer)
        {
            CmdSetCurrentCell(currentCell.GetCellNumber()); // Update cell number on server
        }
        
        var nextCellPosition = nextCell.transform.position;
        var midPoint = (transform.position + nextCell.transform.position) / 2f;
        var hasHitMidpoint = false;
        var midwayPoint = Vector3.Distance(transform.position, nextCellPosition) / 2;

        // Animation Triggers
        if ((transform.position.x > nextCellPosition.x) && transform.position.y > nextCellPosition.y) // LEFT
        {
            playerAnimator.SetTrigger("faceLeft");
        }
        else if ((transform.position.x < nextCellPosition.x) && transform.position.y > nextCellPosition.y) // DOWN
        {
            playerAnimator.SetTrigger("faceDown");
        }
        else if ((transform.position.x > nextCellPosition.x) && transform.position.y < nextCellPosition.y) // UP
        {
            playerAnimator.SetTrigger("faceUp");
        }
        else if ((transform.position.x < nextCellPosition.x) && transform.position.y < nextCellPosition.y) // RIGHT
        {
            playerAnimator.SetTrigger("faceRight");
        }


        while (transform.position != nextCellPosition)
        {
            // Check if player has past midpoint, once they have then move them to the new cell
            if ((Vector3.Distance(transform.position, nextCellPosition) < midwayPoint) && (hasHitMidpoint == false))
            {
                MoveToCell();
                hasHitMidpoint = true;
            }

            var nextPosition = Vector3.MoveTowards(transform.position, nextCell.transform.position, movementSpeed * Time.deltaTime);
            transform.position = nextPosition;
            yield return null;
        }

        MoveToCell(); // for sprite sorting this should happen halfway through
        CompletePath();
    }

    private void MoveToCell()
    {
        transform.parent = nextCell.transform;
        transform.SetSiblingIndex(1);
    }

    /// <summary>
    /// Checks if pathing should continue or if it should stop.
    /// </summary>
    private void CompletePath()

    {
        // If there are still cells in pathing, then keep going. Otherwise stop.
        if (currentPath.Count != 0)
        {
            PerformPathing();
        }
        else
        {
            isPathing = false;
        }

    }

    Vector3 TwoDToIso(Vector3 oldCoordinates)
    {
        var newCoordinates = new Vector2();
        newCoordinates.x = oldCoordinates.x - oldCoordinates.y;
        newCoordinates.y = (oldCoordinates.x + oldCoordinates.y) / 2;
        return newCoordinates;
    }

    Cell FindCellWithCoordinates(Vector3 coordinates)
    {
        var cells = FindObjectsOfType<Cell>();
        foreach (Cell cell in cells)
        {
            if (cell.transform.position == coordinates)
            {
                //print("Found the new cell - " + cell.name);
                return cell;
            }
        }

        return currentCell;
    }
}
