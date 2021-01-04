using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    private class PathFinderGridCell
    {
        public Cell cell;
        public Cell exploredFrom;
        public bool isExplored = false;

        public PathFinderGridCell(Cell cell)
        {
            this.cell = cell;
        }
    }

    public Cell startCell, endCell;

    Dictionary<Vector2, PathFinderGridCell> grid = new Dictionary<Vector2, PathFinderGridCell>();
    [SerializeField] Queue<Cell> queue = new Queue<Cell>();
    bool isRunning = false;
    [SerializeField] Cell searchCenter;
    private List<Cell> path = new List<Cell>();



    Vector2[] directions =
    {
        new Vector2((float) -28, (float) 14), // up
        new Vector2((float) 28, (float) -14), // down
        new Vector2((float) 28, (float) 14), // right
        new Vector2((float) -28, (float)-14) // left

    };

    public void SetStartingCells(Cell startCell, Cell endCell)
    {
        this.startCell = startCell;
        this.endCell = endCell;
    }

    public List<Cell> GetPath()
    {
        Debug.Log("Getting path on client");
        CalculatePath();
        return path;
    }

    private List<Cell> CalculatePath()
    {
        LoadBlocks();
        BreadthFirstSearch();
        CreatePath();
        return path;
    }

    //private void RemoveExploredPath()
    //{
    //    var cells = FindObjectsOfType<Cell>();
    //    foreach (Cell cell in cells)
    //    {
    //        cell.isExplored = false;
    //        cell.exploredFrom = null;
    //    }

    //}

    private void CreatePath()
    {
        Vector2 endCellCoords = endCell.GetCoordinates();

        if (grid[endCellCoords].isExplored == false)
        {
            return;
        }
        SetAsPath(endCell);
        Cell previous = grid[endCellCoords].exploredFrom;

        while (previous != startCell && endCell != startCell)
        {
            SetAsPath(previous);
            previous = grid[previous.GetCoordinates()].exploredFrom;

        }

        SetAsPath(startCell);
        path.Reverse();
    }

    private void SetAsPath(Cell Cell)
    {
        path.Add(Cell);
    }

    private void BreadthFirstSearch()
    {
        queue.Enqueue(startCell);

        while (queue.Count > 0 && isRunning)
        {
            searchCenter = queue.Dequeue();
            HaltIfEndFound();
            ExploreNeighbours();
            grid[searchCenter.GetCoordinates()].isExplored = true;
        }
    }

    private void HaltIfEndFound()
    {
        if (searchCenter == endCell)
        {
            isRunning = false;
        }
    }

    private void ExploreNeighbours()
    {
        if (!isRunning) { return; }


        foreach (Vector2 direction in directions)
        {

            Vector2 neighbourCoordinates = searchCenter.GetCoordinates() + direction;
            if (grid.ContainsKey(neighbourCoordinates))
            {
                QueueNewNeighbours(neighbourCoordinates);

            }
        }
    }

    private void QueueNewNeighbours(Vector2 neighbourCoordinates)
    {
        PathFinderGridCell neighbour = grid[neighbourCoordinates];
        if (neighbour.isExplored || queue.Contains(neighbour.cell))
        {
            // do nothing
        }
        else
        {
            queue.Enqueue(neighbour.cell);
            neighbour.exploredFrom = searchCenter;
        }
    }

    private void LoadBlocks()
    {
        isRunning = true;
        path.Clear();
        grid.Clear();
        queue.Clear();
        searchCenter = startCell;


        var Cells = FindObjectsOfType<Cell>();

        foreach (Cell cell in Cells)
        {
            var gridPos = cell.GetCoordinates();

            if (grid.ContainsKey(gridPos) || !cell.GetIsWalkable())
            {
                // Dont add block
            }
            else
            {
                grid.Add(cell.GetCoordinates(), new PathFinderGridCell(cell));
                //print($"adding cell {Cell.name}");
            }

        }
    }


}
