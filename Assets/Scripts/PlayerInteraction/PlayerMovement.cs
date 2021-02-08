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
    [SerializeField] Player playerInformation;
    CellsManager cellsManager;
    Animator playerAnimator;
    CellObject cellObject;



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
    }

    private void Start()
    {
        // Place at where server says current cell is
        // currentCell = FindCellWithNumber(currentCellNumber);
        cellObject.UpdateCell(currentCell);
        cellObject.FixPositionToCell();

        // Start pathing if player is currently moving
        if (currentCellNumber != destinationCellNumber)
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

        if (currentCellNumber == mouseHoveror.currentCell.GetCellNumber()) { return; }

        destinationCellNumber = mouseHoveror.currentCell.GetCellNumber();

        WebSocketManager.MovementData movementRequest = new WebSocketManager.MovementData(webSocketManager.myPlayerInformation.PlayerNumber, destinationCellNumber);
        WebSocketManager.MessageContainer messageContainer = new WebSocketManager.MessageContainer(WebSocketManager.MessageType.Movement, JsonUtility.ToJson(movementRequest));
        webSocketManager.ws.Send(Encoding.UTF8.GetBytes(JsonUtility.ToJson(messageContainer)));
    }

   /// [Client]
    public void HandleDestinationCellChanged(int newCell)
    {
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
                if (GetComponent<Player>().PlayerNumber == webSocketManager.myPlayerInformation.PlayerNumber)
                {
                    webSocketManager.myPlayerInformation.OnCell = nextCell.cellNumber;
                    WebSocketManager.MessageContainer updateInformationMessageContainer = new WebSocketManager.MessageContainer(WebSocketManager.MessageType.UpdateInformation, 
                    JsonUtility.ToJson(FindObjectOfType<WebSocketManager>().myPlayerInformation));
                    webSocketManager.ws.Send(Encoding.UTF8.GetBytes(JsonUtility.ToJson(updateInformationMessageContainer)));
                }

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
