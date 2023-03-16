using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public int sizeX = 5;
    public int sizeY = 5;

    public int chunkHeight = 5;
    public int chunkWidth = 5;
    

    public bool showAllPoints = false;
    public bool neghbourVision = false;
    //public Vector2Int indexChunk = new Vector2Int();

    Vector2[,] grid = new Vector2[40, 30];

    List<List<Vector2>> chunks = new List<List<Vector2>>();
    public int indexChunk = 0;


    // Start is called before the first frame update
    void Start()
    {
        grid = new Vector2[sizeX, sizeY];

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                grid[x, y] = new Vector2(x, y);
            }
        }

        for (int row = 0; row < sizeX; row += chunkHeight)
        {
            // loop through the columns of the 2D array in increments of the chunk width
            for (int col = 0; col < sizeY; col += chunkWidth)
            {
                // create a new chunk array with the specified width and height
                var chunk = new List<Vector2>();

                // loop through the elements in the chunk and copy the corresponding elements from the 2D array
                for (int i = 0; i < chunkHeight; i++)
                {
                    for (int j = 0; j < chunkWidth; j++)
                    {
                        if (row + i < grid.GetLength(0) && col + j < grid.GetLength(1))
                            chunk.Add(grid[row + i, col + j]);
                    }
                }

                // add the chunk to the list
                chunks.Add(chunk);
            }
        }
    }


    private void OnDrawGizmos()
    {
        if (chunks.Count > 0) 
        {
            if (showAllPoints)
            {
                Gizmos.color = Color.blue;

                for (int x = 0; x < sizeX; x++)
                {
                    for (int y = 0; y < sizeY; y++)
                    {
                        Gizmos.DrawSphere(grid[x, y], 0.1f);
                    }
                }
            }
            else
            {
                Gizmos.color = Color.red;
                foreach (var point in chunks[indexChunk])
                {
                    Gizmos.DrawSphere(point, 0.1f);
                }


                if (neghbourVision)
                {
                    Gizmos.color = Color.green;
                    var listOfIndexes = new List<int>();

                    int chunkRow = indexChunk / (sizeX / chunkWidth);
                    int chunkCol = indexChunk % (sizeY / chunkHeight);

                    listOfIndexes.Add(indexChunk - 1);
                    listOfIndexes.Add(indexChunk + 1);

                    listOfIndexes.Add((chunkRow - 1) * (sizeX / chunkWidth) + chunkCol);   // upinde
                    listOfIndexes.Add((chunkRow + 1) * (sizeY / chunkWidth) + chunkCol);

                    listOfIndexes.Add(listOfIndexes[3] - 1);
                    listOfIndexes.Add(listOfIndexes[3] + 1);
                    //listOfIndexes.Add(listOfIndexes[2] - 1);
                    //listOfIndexes.Add(listOfIndexes[2] + 1);

                    foreach (var indexes in listOfIndexes)
                    {
                        if (indexes < chunks.Count && indexes >= 0)
                        {
                            foreach (var point in chunks[indexes])
                            {
                                Gizmos.DrawSphere(point, 0.1f);
                            }
                        }
                    }







                }
            }
        }
        

      
    }
}
