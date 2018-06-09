using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class GridPlacer : EditorWindow
{

    private GameObject gridDotPrefab = null;
    private GameObject grid = null;
    private int rows = 8; // needs to be even
    private int columns = 16; // needs to be even
    private float gridStep = 9f;

    private GameObject tempObj = null;
    private GameObject[,] placedGrid = null;

    private Vector3 tempPos = Vector3.zero;
    private float tempOffset = 0f;

    [MenuItem("Window/GridPlacer")]
    static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(GridPlacer));

        
    }

    void OnInspectorUpdate()
    {
        Repaint();

        placedGrid = new GameObject[rows, columns];
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Flo's Grid Drawer", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        grid = EditorGUILayout.ObjectField("Grid GameObject:", grid, typeof(GameObject), true) as GameObject;
        gridDotPrefab = EditorGUILayout.ObjectField("GridDot Prefab:", gridDotPrefab, typeof(GameObject), true) as GameObject;
        rows = EditorGUILayout.IntField("Number of Rows: ", rows);
        columns = EditorGUILayout.IntField("Number of Columns: ", columns);
        gridStep = EditorGUILayout.FloatField("Dot Padding: ", gridStep);

        EditorGUILayout.Space();

        // the dot prefab was referenced
        if (gridDotPrefab && grid)
        {
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

                        placedGrid[i+ (rows / 2), j + (columns / 2)] = tempObj;
                        tempObj = null;
                    }
                }
            }

            if (GUILayout.Button("Activate only visible Dots"))
            {
                // remove the old grid
                var tempList = grid.transform.Cast<Transform>().ToList();
                foreach (var child in tempList)
                {
                    Vector3 pos = Camera.main.WorldToViewportPoint(child.transform.position);

                    // if the dot lays outside the viewport and outside the treshhold, deactivate it
                    float t = 0.005f; // treshhold
                    if (pos.x < 0-t || pos.x > 1 + t || pos.y < 0 - t || pos.y > 1 + t)
                        child.gameObject.SetActive(false);
                    else
                        child.gameObject.SetActive(true);
                }
            }

            if (GUILayout.Button("Activate all Dots"))
            {
                // remove the old grid
                var tempList = grid.transform.Cast<Transform>().ToList();
                foreach (var child in tempList)
                {
                    child.gameObject.SetActive(true);
                }
            }

            if (GUILayout.Button("Deactivate all Dots"))
            {
                // remove the old grid
                var tempList = grid.transform.Cast<Transform>().ToList();
                foreach (var child in tempList)
                {
                    child.gameObject.SetActive(false);
                }
            }


            if (GUILayout.Button("Clear the Grid"))
            {
                var tempList = grid.transform.Cast<Transform>().ToList();
                foreach (var child in tempList)
                {
                    DestroyImmediate(child.gameObject);
                }
            }
        }
        else
        {
            EditorGUILayout.Space();
            GUI.contentColor = Color.red;
            EditorGUILayout.LabelField("You must reference the dot prefab as well as the grid.");
        }
    }
}
