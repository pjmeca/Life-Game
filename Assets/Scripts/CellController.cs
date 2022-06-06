using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

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

    // Parallelization vars
    public const int N_THREADS = 4;
    private Thread[] t;
    private int portionSize, mitad;
    private bool threadsRunning = false;

    // Start is called before the first frame update
    void Awake()
    {
        // Get population size and update it if its not exact
        size = (int)Mathf.Sqrt(populationSize);
        populationSize = size * size;

        // Create children
        matrix = new CellMatrix(size, prefab, this.transform);

        // Create Threads
        Debug.Assert(N_THREADS%2 == 0);
        t = new Thread[N_THREADS];
        mitad = N_THREADS/2;
        portionSize = size/mitad;
    }

    // Update is called once per frame
    void Update()
    {
        // We're gona parallelize the status update
        if (!threadsRunning)
        {
            int begin_i, end_i, begin_j, end_j;

            for (int i = 0; i < N_THREADS; i++)
            {
                if(i < mitad){
                    begin_i = 0;
                    end_i = size/2;
                    begin_j = i * portionSize;
                    end_j = (i+1) * portionSize;
                }else {
                    begin_i = size/2;
                    end_i = size;
                    begin_j = (i-mitad) * portionSize;
                    end_j = (i+1-mitad) * portionSize;
                }

                // The last thread must cover the leftovers
                if (end_j < size && (i+2) * portionSize > size)
                    end_j = size;

                t[i] = new Thread(() => matrix.updateCellsStatusParallel(begin_i, end_i, begin_j, end_j));
                t[i].Start();
            }

            threadsRunning = true;
        }

        /*
        Its important to check first all the cells and then update then, that way
        we make sure an update in a cycle does not interfere with the status of
        the rest of the cells in that same cycle.
        */
        speedCounter += speed * Time.deltaTime;
        if (speedCounterInt != (int)speedCounter)
        {
            speedCounterInt = (int)speedCounter;

            //matrix.updateCellsStatus();

            // Wait for all the threads
            foreach (Thread thread in t){
                thread.Join();
                Debug.Assert(!thread.IsAlive);
            }
            threadsRunning = false;

            matrix.updateCellsParallel();
        }
    }

    public int getSize()
    {
        return size;
    }
}