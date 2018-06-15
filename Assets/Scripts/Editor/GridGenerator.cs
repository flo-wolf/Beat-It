using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class GridGenerator : EditorWindow
{

    private GameObject gridDotPrefab = null;
    private GameObject grid = null;
    private int rows = 8; // needs to be even
    private int columns = 16; // needs to be even
    private int gridStep = 8;

    private GameObject tempObj = null;
    private GridDot[,] placedDots = null;

    private Vector3 tempPos = Vector3.zero;
    private float tempOffset = 0f;

    [MenuItem("Window/GridGenerator")]
    static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(GridGenerator));
    }

    void OnInspectorUpdate()
    {
        Repaint();

        placedDots = new GridDot[rows, columns];
    }

    // updates the Dot array in the Grid class
    private void UpdateDotReferences()
    {
        Grid g = grid.GetComponent<Grid>();
        if(g != null)
        {
            g.rows = rows;
            g.columns = columns;
            g.gridStep = (int) gridStep;
        }
        else // the grid component doesnt exist
        {
            Debug.LogError("GridWindow: The referenced Grid-GameObject doesn't have the Grid script attached.");
        }
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Grid Generator", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if (grid == null)
        {
            GameObject[] grids;
            grids = GameObject.FindGameObjectsWithTag("Grid");
            if (grids.Length > 0)
            {
                grid = grids[0];
            }
        }

        gridDotPrefab = EditorGUILayout.ObjectField("GridDot Prefab:", gridDotPrefab, typeof(GameObject), true) as GameObject;
        rows = EditorGUILayout.IntField("Number of Rows: ", rows);
        columns = EditorGUILayout.IntField("Number of Columns: ", columns);
        gridStep = EditorGUILayout.IntField("Dot Padding: ", gridStep);

        EditorGUILayout.Space();

        // the dot prefab was referenced
        if (gridDotPrefab && grid)
        {

/// Grid Placer
            if (GUILayout.Button("Redraw the Grid"))
            {
                // remove the old grid
                var tempList = grid.transform.Cast<Transform>().ToList();
                foreach (var child in tempList)
                {
                    DestroyImmediate(child.gameObject);
                }

                for (int i = -(rows/2); i < rows/2; i++)
                {
                    for(int j = -(columns/2); j < columns/2; j++)
                    {
                        // calculate the placement position:

                        // row is uneven => offset it to get the hexagonal grid)
                        if (i % 2 == 0)
                            tempOffset = gridStep / 2;
                        else
                            tempOffset = 0f;

                        tempPos = Vector2.zero;
                        tempPos.x += j * gridStep + tempOffset;
                        tempPos.y += i * gridStep;

                        tempObj = (GameObject)Instantiate(gridDotPrefab, tempPos, Quaternion.identity);
                        tempObj.name = "GridDot Row" + (i + (rows / 2)) + " Column" + (j + (columns / 2));
                        tempObj.transform.parent = grid.transform;

                        //update the references in the grid class
                        GridDot dot = tempObj.GetComponent<GridDot>();
                        if(dot != null)
                        {
                            dot.row = i +(rows / 2);
                            dot.column = j + (columns / 2);
                            placedDots[i + (rows / 2), j + (columns / 2)] = dot;
                        }
                        else
                        {
                            EditorGUILayout.Space();
                            Debug.LogError("GridWindow: The GridDot prefab doesnt have a GridDot script attached.");
                            EditorGUILayout.Space();
                        }

                        tempObj = null;
                    }
                }

                UpdateDotReferences();
            }

            if (GUILayout.Button("Activate only visible Dots"))
            {
                BoxCollider2D box = grid.GetComponent<BoxCollider2D>();
                if(box != null)
                {
                    // remove the old grid
                    var tempList = grid.transform.Cast<Transform>().ToList();
                    foreach (var child in tempList)
                    {
                        /*
                        // deactivate all the gameobjects outside of the camera
                        Vector3 pos = Camera.main.WorldToViewportPoint(child.transform.position);

                        // if the dot lays outside the viewport and outside the treshhold, deactivate it
                        float t = 0.005f; // treshhold
                        if (pos.x < 0 - t || pos.x > 1 + t || pos.y < 0 - t || pos.y > 1 + t)
                            child.gameObject.SetActive(false);
                        else
                            child.gameObject.SetActive(true);

                        */

                        // deactivate all the objects outside the bounding box
                        if (box.bounds.Contains(child.transform.position))
                        {
                            child.gameObject.SetActive(true);
                            GridDot dot = child.GetComponent<GridDot>();
                            if(dot != null)
                            {
                                dot.active = true;
                            }
                        }
                        else
                        {
                            GridDot dot = child.GetComponent<GridDot>();
                            if (dot != null)
                            {
                                dot.active = false;
                            }
                            child.gameObject.SetActive(false);
                        }
                    }
                }
            }

            if (GUILayout.Button("Delete all non-visible Dots"))
            {
                BoxCollider2D box = grid.GetComponent<BoxCollider2D>();
                if (box != null)
                {
                    // remove the old grid
                    var tempList = grid.transform.Cast<Transform>().ToList();
                    foreach (var child in tempList)
                    {
                        // delete all the objects outside the bounding box
                        if (!box.bounds.Contains(child.transform.position))
                        {
                            GameObject.DestroyImmediate(child.gameObject);
                        }
                    }
                }
            }

            if (GUILayout.Button("Activate all Dots"))
            {
                // remove the old grid
                var tempList = grid.transform.Cast<Transform>().ToList();
                foreach (var child in tempList)
                {
                    child.gameObject.SetActive(true);
                    GridDot dot = child.GetComponent<GridDot>();
                    if (dot != null)
                    {
                        dot.active = true;
                    }
                }
            }

            if (GUILayout.Button("Deactivate all Dots"))
            {
                // remove the old grid
                var tempList = grid.transform.Cast<Transform>().ToList();
                foreach (var child in tempList)
                {
                    GridDot dot = child.GetComponent<GridDot>();
                    if (dot != null)
                    {
                        dot.active = false;
                    }
                    child.gameObject.SetActive(false);
                }
            }

            if (GUILayout.Button("Clear the Grid, Delete all Dots"))
            {
                var tempList = grid.transform.Cast<Transform>().ToList();
                foreach (var child in tempList)
                {
                    DestroyImmediate(child.gameObject);
                }
                placedDots = new GridDot[rows, columns];
                UpdateDotReferences();
            }
        }
        else
        {
            EditorGUILayout.Space();
            GUI.contentColor = Color.red;
            EditorGUILayout.LabelField("You must reference the dot prefab in order to proceed.");
        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Change the collider size on the Grid GameObject in order to define the visible space.");
    }
}
