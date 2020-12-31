using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] float movementSpeed = 0.5f;

    public Cell currentCell;

    Cell nextCell;

    public List<Cell> currentPath = new List<Cell>();

    public bool isPathing = false;


    // Cached Variables
    Animator playerAnimator;

    void Start()
    {
        playerAnimator = GetComponent<Animator>();
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
