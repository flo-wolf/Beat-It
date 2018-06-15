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

    [HideInInspector]
    public int gridStep = 8; // gets set by the GridPlacer Editor Window when modifying the grid

    // Use this for initialization
    void Start ()
    {
        if (instance == null)
            instance = this;


        gridDots = new GridDot[rows, columns];
        gridDots = BuildDotArray();
	}
	
    // cycle through all childs of this object and append them to our 2D-GridDot-Array, indicating rows and columns.
    public GridDot[,] BuildDotArray()
    {
        GridDot[,] foundDots = new GridDot[rows, columns];

        var temp = GetComponentsInChildren<GridDot>();
        Debug.Log("GridDot length: " + temp.Length);

        foreach (GridDot child in temp)
        {
                if (child != null) // child is a griddot
                {
                    foundDots[child.row, child.column] = child;
                }
        }
        return foundDots;
    }

    public static GridDot FindPlayerSpawn()
    {
        PlayerSpawn s = instance.GetComponentInChildren<PlayerSpawn>();
        if(s != null)
        {
            GridDot parentDot = s.transform.parent.GetComponent<GridDot>();
            return parentDot;
        }
        return null;
    }

    public static GridDot GetRandomActiveDot()
    {
        GridDot g = null;
        int tries = 15;
        for(int i = 0; i < tries; i++)
        {
            g = gridDots[Random.Range(0, instance.rows - 1), Random.Range(0, instance.columns - 1)];
            if (g != null && g.active && g.levelObject == null && g.gameObject.activeSelf)
                return g;
        }
        return g;
    }

    // get the nearest dot based on a normalized input (thumbstick to the right => closest active dot on the right)
	public static GridDot GetNearestActiveDot(GridDot dot, Vector2 direction)
    {

        // gather all six sourrounding dots
        List<GridDot> hexDots = new List<GridDot>();

        if (Input.GetKey(KeyCode.Space))
        {
            //Double Jump
            // our row is uneven
            if (dot.row % 2 == 0)
            {
                // top left dot
                if (dot.row - 2 >= 0)
                    hexDots.Add(gridDots[dot.row - 2, dot.column]);
                // top right dot
                if (dot.row - 2 >= 0 && dot.column + 1 < instance.columns)
                    hexDots.Add(gridDots[dot.row - 2, dot.column]);

                // left dot
                if (dot.column - 2 >= 0)
                    hexDots.Add(gridDots[dot.row, dot.column - 2]);
                // right dot
                if (dot.column + 2 < instance.columns)
                    hexDots.Add(gridDots[dot.row, dot.column + 2]);

                // bot left dot
                if (dot.row + 2 < instance.rows)
                    hexDots.Add(gridDots[dot.row + 2, dot.column]);
                // bot right dot
                if (dot.row + 2 < instance.rows && dot.column + 1 < instance.columns)
                    hexDots.Add(gridDots[dot.row + 2, dot.column]);
            }

            // our row is even
            else
            {
                // top left dot
                if (dot.row - 2 >= 0 && dot.column - 1 >= 0)
                    hexDots.Add(gridDots[dot.row - 2, dot.column]);
                // top right dot
                if (dot.row - 2 >= 0)
                    hexDots.Add(gridDots[dot.row - 2, dot.column]);

                // left dot
                if (dot.column - 2 >= 0)
                    hexDots.Add(gridDots[dot.row, dot.column - 2]);
                // right dot
                if (dot.column + 2 < instance.columns)
                    hexDots.Add(gridDots[dot.row, dot.column + 2]);

                // bot left dot
                if (dot.row + 2 < instance.rows && dot.column - 1 >= 0)
                    hexDots.Add(gridDots[dot.row + 2, dot.column]);
                // bot right dot
                if (dot.row + 2 < instance.rows)
                    hexDots.Add(gridDots[dot.row + 2, dot.column]);
            }
        }

        else
        {
            // Normal movement
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
                    hexDots.Add(gridDots[dot.row - 1, dot.column - 1]);
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
        }

        direction = (Vector2)dot.transform.position + direction;

        //Debug.Log("Nearest Active - centerDot: " + dot.transform.position + " direction: " + direction);

        GridDot closestDot = null;
        float closestLength = 10000000f;

        foreach (GridDot d in hexDots)
        {
            // rule out all the dots that are not set to be active
            if (d != null && d.active && d.gameObject.activeSelf)
            {
                // get distance between the sourrounding hexagon dot and our dot position plus the lookdirection
                float l = (direction - (Vector2)d.transform.position).magnitude;

                // pythagoras fix for row offsets
                if((dot.row % 2 == 0 && d.row % 2 != 0) || (dot.row % 2 != 0 && d.row % 2 == 0))
                {
                    float hypothenuse = Mathf.Sqrt(((instance.gridStep / 2) * (instance.gridStep / 2)) + ((instance.gridStep) * (instance.gridStep)));
                    float multi = instance.gridStep / hypothenuse;
                    l = l * multi;

                }

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
