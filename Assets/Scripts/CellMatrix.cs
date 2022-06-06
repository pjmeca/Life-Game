using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellMatrix
{
    private int size;
    private Transform[][] cells;
    private bool[][] statusParallel; // As we can't access CellScript from Threads, 
                                     // we're gonna temporally store them here
    private Transform prefab;

    public CellMatrix(int size, Transform prefab, Transform parent)
    {
        this.size = size;
        this.prefab = prefab;

        createChildren(parent);
        randomInit();
        updateCells();
    }

    private void createChildren(Transform parent)
    {

        cells = new Transform[size][];
        statusParallel = new bool[size][];

        // Row
        for (int i = 0; i < size; i++)
        {
            cells[i] = new Transform[size];
            statusParallel[i] = new bool[size];

            // Column
            for (int j = 0; j < size; j++)
            {
                Vector3 v = new Vector3(i * prefab.transform.localScale.x, j * prefab.transform.localScale.x, 0);
                cells[i][j] = Object.Instantiate(prefab, v, Quaternion.identity);
                cells[i][j].parent = parent;
            }
        }
    }

    private void randomInit(){
        for(int i=0; i<size; i++)
            for(int j=0; j<size; j++){
                if(Random.Range(0f, 1f) < 0.35f){
                    statusParallel[i][j] = true;
                    get(i,j).setAlive(true);
                } else{
                    statusParallel[i][j] = false;
                    get(i,j).setAlive(false);
                }
            }
        
    }

    // Returns the CellScript in the specified position
    public CellScript get(int i, int j)
    {
        return cells[i][j].GetComponent<CellScript>();
    }
    public bool getParallel(int i, int j){
        return statusParallel[i][j];
    }
    public void setParallel(int i, int j, bool value){
        statusParallel[i][j] = value;
    }

    // Updates the status of all the cells
    public void updateCellsStatus()
    {
        // Check all the cells
        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
            {
                // Checks all neighbors and changes its status if necessary
                int nalive = 0;

                for (int ii = i - 1; ii <= i + 1; ii++)
                    for (int jj = j - 1; jj <= j + 1; jj++)
                        if (!(ii < 0 || ii >= size || jj < 0 || jj >= size) &&
                            !(ii == i && jj == j)) // Avoid current cell
                        {
                            if (get(ii, jj).isAlive())
                                nalive++;
                        }

                // Alive cell needs 2 or 3 neighbors to be alive in order to keep alive
                if (get(i, j).isAlive())
                {
                    if (!(nalive == 2 || nalive == 3))
                        get(i, j).setAlive(false);
                }
                // Dead cell needs 3 alive neighbors to come back to life
                else
                {
                    if (nalive == 3)
                        get(i, j).setAlive(true);
                }
            }
    }

    // Updates the status of all the cells from the square between begin and end (not reached)
    public void updateCellsStatusParallel(int begin_i, int end_i, int begin_j, int end_j)
    {
        // Check all the cells
        for (int i = begin_i; i < end_i; i++)
            for (int j = begin_j; j < end_j; j++)
            {
                // Checks all neighbors and changes its status if necessary
                int nalive = 0;

                for (int ii = i - 1; ii <= i + 1; ii++)
                    for (int jj = j - 1; jj <= j + 1; jj++)
                        if (!(ii < 0 || ii >= size || jj < 0 || jj >= size) &&
                            !(ii == i && jj == j)) // Avoid current cell
                        {
                            if (getParallel(ii, jj))
                                nalive++;
                        }

                // Alive cell needs 2 or 3 neighbors to be alive in order to keep alive
                if (getParallel(i, j))
                {
                    if (!(nalive == 2 || nalive == 3))
                        setParallel(i, j, false);
                }
                // Dead cell needs 3 alive neighbors to come back to life
                else
                {
                    if (nalive == 3)
                        setParallel(i, j, true);
                }
            }
    }

    // Updates all the cells at the same time
    public void updateCells()
    {
        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
            {
                get(i, j).updateCell();
            }
    }
    public void updateCellsParallel(){
        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
            {
                CellScript cell = get(i, j);
                cell.setAlive(statusParallel[i][j]);
                cell.updateCell();
            }
    }
}
