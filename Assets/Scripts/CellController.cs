using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellController : MonoBehaviour
{
    [Range(0f, 50f)]
    public float speed;
    private float speedCounter = 0f;
    private int speedCounterInt = 0;
    public int populationSize = 200;
    public Transform prefab;
    private int size;
    private CellMatrix matrix;

    // Start is called before the first frame update
    void Awake()
    {
        // Get population size and update it if its not exact
        size = (int)Mathf.Sqrt(populationSize);
        populationSize = size * size;

        // Create children
        matrix = new CellMatrix(size, prefab, this.transform);
    }

    // Update is called once per frame
    void Update()
    {
        /*
        Its important to check first all the cells and then update then, that way
        we make sure an update in a cycle does not interfere with the status of
        the rest of the cells in that same cycle.
        */
        speedCounter += speed * Time.deltaTime;
        if (speedCounterInt != (int) speedCounter)
        {
            speedCounterInt = (int) speedCounter;
            matrix.updateCellsStatus();
            matrix.updateCells();
        }
    }

    public int getSize(){
        return size;
    }
}

// Matrix with all the cells (children)
public class CellMatrix
{

    private int size;
    private Transform[][] cells;
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

        // Row
        for (int i = 0; i < size; i++)
        {
            cells[i] = new Transform[size];

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
                    //Debug.Log("Cell is: ALIVE");
                    get(i,j).setAlive(true);
                } else{
                    //Debug.Log("Cell is: DEAD");
                    get(i,j).setAlive(false);
                }
            }

        /*for(int i=0; i<2; i++)
            for(int j=0; j<2; j++){
                get(i,j).setAlive(true);
            }*/
        /*for(int i=0; i<3; i++){
            get(i,1).setAlive(true);
        }*/
        
    }

    // Returns the CellScript in the specified position
    public CellScript get(int i, int j)
    {
        return cells[i][j].GetComponent<CellScript>();
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

    // Updates all the cells at the same time
    public void updateCells()
    {
        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
            {
                get(i, j).updateCell();
            }
    }
}