using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public static Grid instance;
    public static GridDot[,] gridDots;

    [HideInInspector]
    public int rows = -1; // gets set by the GridPlacer Editor Window when modifying the grid
    [HideInInspector]
    public int columns = -1; // gets set by the GridPlacer Editor Window when modifying the grid

    // Use this for initialization
    void Start ()
    {
        if (instance == null)
            instance = this;

        BuildDotArray();
	}
	
    // cycle through all childs of this object and append them to our 2D-GridDot-Array, indicating rows and columns.
    void BuildDotArray()
    {
        gridDots = new GridDot[rows,columns];

        var tempList = transform.Cast<Transform>().ToList();
        foreach (var child in tempList)
        {
            GridDot dot = child.GetComponent<GridDot>();
            if(dot != null) // child is a griddot
            {
                gridDots[dot.row,dot.column] = dot;
            }
        }
    }

    public static GridDot GetRandomActiveDot()
    {
        GridDot g = null;
        int tries = 15;
        for(int i = 0; i < tries; i++)
        {
            g = gridDots[Random.Range(0, instance.rows - 1), Random.Range(0, instance.columns - 1)];
            if (g != null && g.active && g.levelObj == null)
                return g;
        }
        return g;
    }

    // get the nearest dot based on a normalized input (thumbstick to the right => closest active dot on the right)
	public static GridDot GetNearestActiveDot(GridDot dot, Vector2 direction)
    {
        Vector2 aimPos;
        // mouse input
        if (InputDeviceDetector.inputType == InputDeviceDetector.InputType.MouseKeyboard)
        {
            // get the world mouse position 
            Vector2 mousePos = Input.mousePosition;
            aimPos = Camera.main.ScreenToWorldPoint(mousePos);

            aimPos = (aimPos - (Vector2)dot.transform.position).normalized;
        }

        // controller input
        else
        {
            Vector2 controllerInput = new Vector2(Input.GetAxis("Joystick X"), Input.GetAxis("Joystick Y"));

            aimPos = controllerInput;

            // check which point is closest to the lookdirection
            aimPos = (aimPos - (Vector2)dot.transform.position).normalized;
        }

        direction = aimPos;

        //direction = direction;

        // gather all six sourrounding dots
        List<GridDot> hexDots = new List<GridDot>();

        // our row is uneven
        if (dot.row % 2 == 0)
        {
            // top left dot
            if (dot.row - 1 >= 0)
                hexDots.Add(gridDots[dot.row - 1, dot.column]);
            // top right dot
            if (dot.row - 1 >= 0 && dot.column + 1 < instance.columns)
                hexDots.Add(gridDots[dot.row - 1, dot.column + 1]);

            // left dot
            if (dot.column - 1 >= 0)
                hexDots.Add(gridDots[dot.row, dot.column - 1]);
            // right dot
            if (dot.column + 1 < instance.columns)
                hexDots.Add(gridDots[dot.row, dot.column + 1]);

            // bot left dot
            if (dot.row + 1 < instance.rows)
                hexDots.Add(gridDots[dot.row + 1, dot.column]);
            // bot right dot
            if (dot.row + 1 < instance.rows && dot.column + 1 < instance.columns)
                hexDots.Add(gridDots[dot.row + 1, dot.column + 1]);
        }

        // our row is even
        else
        {
            // top left dot
            if (dot.row - 1 >= 0 && dot.column - 1 >= 0)
                hexDots.Add(gridDots[dot.row - 1, dot.column-1]);
            // top right dot
            if (dot.row - 1 >= 0)
                hexDots.Add(gridDots[dot.row - 1, dot.column]);

            // left dot
            if (dot.column - 1 >= 0)
                hexDots.Add(gridDots[dot.row, dot.column - 1]);
            // right dot
            if (dot.column + 1 < instance.columns)
                hexDots.Add(gridDots[dot.row, dot.column + 1]);

            // bot left dot
            if (dot.row + 1 < instance.rows && dot.column - 1 >= 0)
                hexDots.Add(gridDots[dot.row + 1, dot.column - 1]);
            // bot right dot
            if (dot.row + 1 < instance.rows)
                hexDots.Add(gridDots[dot.row + 1, dot.column]);
        }


        Vector2 lookPos = direction - (Vector2)dot.transform.position;
        GridDot closestDot = null;
        float closestLength = 100000f;

        foreach (GridDot d in hexDots)
        {
            // rule out all the dots that are not set to be active
            if (d.active && d.levelObj == null)
            {
                // get distance between the sourrounding hexagon dot and our dot position plus the lookdirection
                float l = (lookPos - (Vector2)d.transform.position).magnitude;
                if(l < closestLength)
                {
                    closestLength = l;
                    closestDot = d;
                }
            }
        }

        return closestDot;
    }
}
