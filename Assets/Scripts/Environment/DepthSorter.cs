
//using UnityEngine;

//[ExecuteInEditMode]
//public class DepthSorter : MonoBehaviour
//{
//    private const int reservedSortingLayersPerCell = 10; // Used to reserve sorting layers within each 

//    void Update()
//    {
//        // Find the cells, sort them, then apply their order in layers
//        Cell[] cells = FindObjectsOfType<Cell>();
//        DepthSorter.DepthSort(cells);
//    }

//    /// <summary>
//    /// Uses a selection sort to sort cells and assigns the cells those layers within each sort cycle
//    /// </summary>
//    /// <param name="cells"></param>
//    static public void DepthSort(Cell[] cells)
//    {
//        int n = cells.Length;

//        // one by one move boundary of unsorted array
//        for (int i = 0; i < n; i++)
//        {
//            // find min in unsorted array
//            int minindex = i;
//            for (int j = i + 1; j < n; j++)
//            {
//                if (cells[j].transform.position.y > cells[minindex].transform.position.y)
//                {
//                    minindex = j;
//                } else if ( (cells[j].transform.position.y ==  cells[minindex].transform.position.y) && cells[j].transform.position.x > cells[minindex].transform.position.x)
//                {
//                    minindex = j;
//                }
//            }

//            // swap found minimum with the first element
//            Cell temp = cells[minindex];
//            cells[minindex] = cells[i];
//            cells[i] = temp;

//            // Assign minimum cell its sorting layer
//            //cells[i].AssignObjectOrderLayers(i * reservedSortingLayersPerCell);
           
//        }

//    }
//}
