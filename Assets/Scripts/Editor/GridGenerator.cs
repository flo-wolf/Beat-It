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

    [MenuItem("Custom/Grid Generator")]
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
        //rows = EditorGUILayout.IntField("Number of Rows: ", rows);
        //columns = EditorGUILayout.IntField("Number of Columns: ", columns);

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Keep this at 8 for all levels", EditorStyles.boldLabel);
        gridStep = EditorGUILayout.IntField("Dot Padding: ", gridStep);

        EditorGUILayout.Space();

        // the dot prefab was referenced
        if (gridDotPrefab && grid)
        {

/// Grid Placer
            if (GUILayout.Button("Fill Box Collider with GridDots"))
            {
                
                BoxCollider2D box = grid.GetComponent<BoxCollider2D>();
                if (box != null)
                {
                    // calculate rows and columns
                    float width = box.size.x;
                    float height = box.size.y;

                    rows = ((int)height / gridStep) + 2;
                    if (rows % 2 != 0)
                        rows += 1;
                    rows = (int) ((float)rows * 2f);
                    columns = ((int)width / gridStep) + 2;
                    if (columns % 2 != 0)
                        columns += 1;
                    columns = (int)((float)columns * 1.5f);


                    // remove the old grid
                    var t = grid.transform.Cast<Transform>().ToList();
                    foreach (var c in t)
                    {
                        DestroyImmediate(c.gameObject);
                    }
                    placedDots = new GridDot[rows, columns];
                    UpdateDotReferences();

                    for (int i = -(rows / 2); i < rows / 2; i++)
                    {
                        for (int j = -(columns / 2); j < columns / 2; j++)
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

                            tempObj = (GameObject)PrefabUtility.InstantiatePrefab(gridDotPrefab);
                            tempObj.transform.position = tempPos;
                            tempObj.name = "GridDot Row" + (i + (rows / 2)) + " Column" + (j + (columns / 2));
                            tempObj.transform.parent = grid.transform;

                            //update the references in the grid class
                            GridDot dot = tempObj.GetComponent<GridDot>();
                            if (dot != null)
                            {
                                dot.row = i + (rows / 2);
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



                    var temp = grid.transform.Cast<Transform>().ToList();
                    foreach (var child in temp)
                    {

                        // delete all the objects outside the bounding box
                        if (!box.bounds.Contains(child.transform.position))
                        {
                            GameObject.DestroyImmediate(child.gameObject);
                        }
                    }
                }

                UpdateDotReferences();
            }

            /*
            if (GUILayout.Button("Activate only visible Dots"))
            {
                BoxCollider2D box = grid.GetComponent<BoxCollider2D>();
                if(box != null)
                {
                    // remove the old grid
                    var tempList = grid.transform.Cast<Transform>().ToList();
                    foreach (var child in tempList)
                    {

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

            if (GUILayout.Button("Delete all non-activated Dots"))
            {
                var tempList = grid.transform.Cast<Transform>().ToList();
                Debug.Log("templisttttttttttttttttttt: " + tempList.Count);
                foreach (Transform t in tempList)
                {
                    GridDot child = t.GetComponent<GridDot>();
                    if(child != null)
                    {
                        // delete all the objects that arent activated
                        if (!child.active || !child.gameObject.activeInHierarchy || !child.gameObject.activeSelf)
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
            */

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
