using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    
    [SerializeField] int destinationCellNumber;
    [SerializeField] int currentCellNumber;

    // Movement
    [SerializeField] float movementSpeed = 50f;
    PathFinder pathfinder;
    public Cell currentCell;
    Cell nextCell;
    public List<Cell> currentPath;
    public bool isPathing = false;

    // Caches
    [SerializeField] MouseHoveror mouseHoveror;
    [SerializeField] Player playerInformation;
    CellsManager cellsManager;
    Animator playerAnimator;
    CellObject cellObject;

    #region Server

    //   [Server]
    public void OnEnterRoom()
    {
        // Spawn player on spawn cell

        currentCellNumber = FindObjectOfType<Spawn>().GetComponent<Cell>().cellNumber;
        this.transform.position = FindObjectOfType<Spawn>().transform.position;
    }

    public void OnStopServer()
    {
        // Remove player from its cell
        cellObject.RemoveFromCell();
    }

    public void CmdSetDestinationCell(int cellNumber)
    {
        // TODO:  Add some type of verification here
        destinationCellNumber = cellNumber;
    }

    public void CmdSetCurrentCell(int cellNumber)
    {
        // Used to sync where local player sets their cell at
        currentCellNumber = cellNumber;
    }
    #endregion

    #region Client

    private void Awake()
    {
        cellsManager = FindObjectOfType<CellsManager>();

        // Set so that default value is spawn
     //   destinationCellNumber = FindObjectOfType<Spawn>().GetComponent<Cell>().cellNumber;

        // Set cached vars
        mouseHoveror = FindObjectOfType<MouseHoveror>();
        pathfinder = FindObjectOfType<PathFinder>();
        playerAnimator = GetComponent<Animator>();
        cellObject = GetComponent<CellObject>();
    }

    private void Start()
    {
        // Place at where server says current cell is
    //    currentCell = FindCellWithNumber(currentCellNumber);
        cellObject.UpdateCell(currentCell);
        cellObject.FixPositionToCell();

        // Start pathing if player is currently moving
  //      if (currentCellNumber != destinationCellNumber)
        {
            pathfinder.SetStartingCells(currentCell, FindCellWithNumber(destinationCellNumber));
            currentPath = pathfinder.GetPath();
            if (isPathing) { return; }
            PerformPathing();
        }
    }

   // [ClientCallback]
    private void Update()
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame) { return; }

        if (mouseHoveror.currentCell == null) { return; }

        if (!mouseHoveror.currentCell.GetIsWalkable()) { return; }

   //     CmdSetDestinationCell(mouseHoveror.currentCell.GetCellNumber());
    }

   /// [Client]
    public void HandleDestinationCellChanged(int oldCell, int newCell)
    {
        // For some reason, even though this is a hook and shouldnt be run on the server I have to check if its a client otherwise it wont deserialize properly
       // if (isClient)
        {
            pathfinder.SetStartingCells(currentCell, FindCellWithNumber(newCell));
            currentPath = pathfinder.GetPath();
            if (isPathing) { return; }
            PerformPathing();
        }
    }

   /// [Client]
    public void HandleCurrentCellChanged(int oldcell, int newCell)
    {
        // cell changed
    }

    #region Player Movement 

  //  [Client]
    public void PerformPathing()
    {

        if (currentPath.Count == 0) { return; }

        isPathing = true;
        nextCell = currentPath[0];
        currentPath.RemoveAt(0);

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

   // [Client]
    private IEnumerator PerformMovement(Cell nextCell)
    {

        currentCell = nextCell; // set current cell here since the movement will be completed - otherwise pathfinder may still include previous cell on multiple clicks

      //  if (isLocalPlayer)
        {
   //         CmdSetCurrentCell(currentCell.GetCellNumber()); // Update cell number on server
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
                cellObject.UpdateCell(nextCell);
                hasHitMidpoint = true;
            }

            var nextPosition = Vector3.MoveTowards(transform.position, nextCell.transform.position, movementSpeed * Time.deltaTime);
            transform.position = nextPosition;
            yield return null;
        }

        //cellObject.UpdateCell(nextCell); // for sprite sorting this should happen halfway through
        cellObject.FixPositionToCell();
        CompletePath();
    }

    private void MoveToCell()
    {
        transform.parent = nextCell.transform;
        transform.SetSiblingIndex(1);
    }

    // Checks if pathing should continue or stop
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

    #endregion

    #endregion

    #region Helper Functions

    private Cell FindCellWithNumber(int cellNumber)
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


    // Used currently to ensure start is run late (After all starts without the init coroutine)
    IEnumerator InitCoroutine()
    {
        yield return new WaitForEndOfFrame();
    }

    Cell FindCellWithCoordinates(Vector3 coordinates)
    {
        var cells = FindObjectsOfType<Cell>();
        foreach (Cell cell in cells)
        {
            if (cell.transform.position == coordinates)
            {
                return cell;
            }
        }

        return currentCell;
    }

    Vector3 TwoDToIso(Vector3 oldCoordinates)
    {
        var newCoordinates = new Vector2();
        newCoordinates.x = oldCoordinates.x - oldCoordinates.y;
        newCoordinates.y = (oldCoordinates.x + oldCoordinates.y) / 2;
        return newCoordinates;
    }

    #endregion

}
