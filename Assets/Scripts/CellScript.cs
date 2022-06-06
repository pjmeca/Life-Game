using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellScript : MonoBehaviour
{
    private Renderer cellRenderer;
    public enum Status
    {
        ALIVE, DEAD, NULL
    }
    public Status status = Status.DEAD;    // Current cell status
    private Status nextStatus = Status.NULL;// The status that will be set in the next cycle
    public Color aliveColor = Color.white;  // Default color when cell is alive
    public Color deadColor = Color.black;   // Default color when cell is dead

    void Awake()
    {
        cellRenderer = GetComponent<Renderer>();
        updateCell();
    }

    // Change color depending on cell state
    public void updateCell()
    {
        // If the status has not changed, we can return
        if (nextStatus == status)
            return;

        // Update the status
        if(nextStatus != Status.NULL)
            status = nextStatus;

        // Set colors accordingly
        if (isAlive())
        {
            cellRenderer.material.SetColor("_Color", aliveColor);
        }
        else
        {
            cellRenderer.material.SetColor("_Color", deadColor);
        }
    }

    // Changes next status
    public void setAlive(bool alive)
    {
        if (alive)
            nextStatus = Status.ALIVE;
        else
            nextStatus = Status.DEAD;
    }

    // Returns whether or not the cell is alive
    public bool isAlive()
    {
        return status == Status.ALIVE;
    }
}
