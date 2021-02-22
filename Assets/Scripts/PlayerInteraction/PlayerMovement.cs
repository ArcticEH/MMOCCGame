using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Text;

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
    [SerializeField] WebSocketManager webSocketManager;
    [SerializeField] MouseHoveror mouseHoveror;
    CellsManager cellsManager;
    Animator playerAnimator;
    CellObject cellObject;
    NetworkPlayer networkPlayer;


    private void Awake()
    {
        cellsManager = FindObjectOfType<CellsManager>();

        // Set so that default value is spawn
        destinationCellNumber = FindObjectOfType<Spawn>().GetComponent<Cell>().cellNumber;

        // Set cached vars
        webSocketManager = FindObjectOfType<WebSocketManager>();
        mouseHoveror = FindObjectOfType<MouseHoveror>();
        pathfinder = FindObjectOfType<PathFinder>();
        playerAnimator = GetComponent<Animator>();
        cellObject = GetComponent<CellObject>();
        networkPlayer = GetComponent<NetworkPlayer>();

    }

    private void Start()
    {
    }

    public void SpawnPlayer(SpawnResponse existingSpawnData)
    {
        // TODO: Get player direction facing

        currentCell = FindCellWithNumber(existingSpawnData.cellNumber);
        cellObject.UpdateCell(FindCellWithNumber(existingSpawnData.sortingCellNumber));
        cellObject.FixPositionToCell();

        // Update transform position
        transform.position = new Vector3(existingSpawnData.xPosition, existingSpawnData.yPosition, transform.position.z);
    }

    private void Update()
    {

        // Determine if valid movement request was received

        if (networkPlayer.Id != webSocketManager.localNetworkPlayerId) { return;  } // Only check if this is the local player

        if (!Mouse.current.leftButton.wasPressedThisFrame) { return; }

        if (mouseHoveror.currentCell == null) { return; }

        if (!mouseHoveror.currentCell.GetIsWalkable()) { return; }


        if (currentCellNumber == mouseHoveror.currentCell.GetCellNumber()) { return; }


        // Create movement request passage from pathfind

        // Get path
        pathfinder.SetStartingCells(currentCell, mouseHoveror.currentCell);
        currentPath = pathfinder.GetPath();

        // Create values for message
        int[] cellNumbers = new int[currentPath.Count];
        int[] cellXValues = new int[currentPath.Count];
        int[] cellYValues = new int[currentPath.Count];
        for (int i = 0; i < cellNumbers.Length; i++)
        {
            Cell cell = currentPath[i];
            cellNumbers[i] = cell.cellNumber;
            cellXValues[i] = (int)cell.transform.position.x;
            cellYValues[i] = (int)cell.transform.position.y;
        }

        // Send message to server
        MovementDataRequest movementDataRequest = new MovementDataRequest
        {
            playerId = networkPlayer.Id,
            cellNumberPath = cellNumbers,
            cellPathXValues = cellXValues,
            cellPathYValues = cellYValues
        };

        webSocketManager.SendMessage(MessageType.MovementDataRequest, JsonUtility.ToJson(movementDataRequest));

    }

    public void HandlePlayerPositionChanged(MovementDataUpdate movementDataUpdate)
    {

        // Update current cell number if different
        if (movementDataUpdate.cellNumber != currentCellNumber)
        {
            // Determine direction that player is currrently facing
            // Animation Triggers
            if ((transform.position.x > movementDataUpdate.xPosition) && transform.position.y > movementDataUpdate.yPosition) // LEFT
            {
                playerAnimator.SetTrigger("faceLeft");
            }
            else if ((transform.position.x < movementDataUpdate.xPosition) && transform.position.y > movementDataUpdate.yPosition) // DOWN
            {
                playerAnimator.SetTrigger("faceDown");
            }
            else if ((transform.position.x > movementDataUpdate.xPosition) && transform.position.y < movementDataUpdate.yPosition) // UP
            {
                playerAnimator.SetTrigger("faceUp");
            }
            else if ((transform.position.x < movementDataUpdate.xPosition) && transform.position.y < movementDataUpdate.yPosition) // RIGHT
            {
                playerAnimator.SetTrigger("faceRight");
            }

            Cell cell = FindCellWithNumber(movementDataUpdate.cellNumber);       
            currentCellNumber = cell.cellNumber;
            currentCell = cell;

        }

        // Update sorting cell number if different
        if (movementDataUpdate.sortingCellNumber != cellObject.myCell.cellNumber)
        {
            Cell cell = FindCellWithNumber(movementDataUpdate.sortingCellNumber);
            cellObject.UpdateCell(cell);
        }

        // Update transform position
        transform.position = new Vector3(movementDataUpdate.xPosition, movementDataUpdate.yPosition, transform.position.z);

    }


    #region Helper Functions

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

    public static Cell FindCellWithNumberTwo(int cellNumber)
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



///// [Client]
// public void HandleCurrentCellChanged(int oldcell, int newCell)
// {
//     // cell changed
// }


//public void HandleDestinationCellChanged(int newCell)
//{
//    {
//        pathfinder.SetStartingCells(currentCell, FindCellWithNumber(newCell));
//        currentPath = pathfinder.GetPath();
//        if (isPathing) { return; }
//        PerformPathing();
//    }
//}

////  [Client]
//  public void PerformPathing()
//  {

//      if (currentPath.Count == 0) { return; }

//      isPathing = true;
//      nextCell = currentPath[0];
//      currentPath.RemoveAt(0);

//      // Determine which direction this path is in
//      if (nextCell == currentCell)
//      {
//          CompletePath();
//      }
//      else
//      {
//          StartCoroutine(PerformMovement(nextCell));
//      }
//  }


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

//    System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
//    stopWatch.Start();
//    int timesMoved = 0;
//    float distanceTravelled = 0f;
//    while (transform.position != nextCellPosition)
//    {



//        // Check if player has past midpoint, once they have then move them to the new cell
//        if ((Vector3.Distance(transform.position, nextCellPosition) < midwayPoint) && (hasHitMidpoint == false) || deltaTime > 0.5)
//        {
//            // TODO: send to server that player has actually switched positions here
//            cellObject.UpdateCell(nextCell);
//            hasHitMidpoint = true;
//        }
//        timesMoved++;
//        //print($"Delta time - {deltaTime}");

//        Vector3 nextPosition;
//        nextPosition = Vector3.MoveTowards(transform.position, nextCell.transform.position, movementSpeed * deltaTime);

//        // Update distance travelled 
//        distanceTravelled += (Vector3.Distance(transform.position, nextPosition));
//        //print($"Distance travelled = {distanceTravelled}");


//        transform.position = nextPosition;
//        yield return null;
//    }
//    stopWatch.Stop();
//    Debug.Log("Time to move to cell: " + stopWatch.ElapsedMilliseconds + " In steps - " + timesMoved);

//    //cellObject.UpdateCell(nextCell); // for sprite sorting this should happen halfway through
//    cellObject.FixPositionToCell();
//    CompletePath();
//}

//private void MoveToCell()
//{
//    transform.parent = nextCell.transform;
//    transform.SetSiblingIndex(1);
//}

//// Checks if pathing should continue or stop
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



