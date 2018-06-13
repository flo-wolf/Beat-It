using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class LevelObjectPlacer : EditorWindow
{

    private LevelObject.Type objectType = LevelObject.Type.Unset;
    private enum PlaceMode { None, Place, Remove }
    private PlaceMode mode = PlaceMode.None;

    private GameObject grid = null;
    private GridDot[,] gridDots = null;
    private Vector3 tempPos = Vector3.zero;

    // prefabs
    private GameObject playerSpawnPrefab = null;
    private GameObject killDotPrefab = null;

    [MenuItem("Window/LevelObjectPlacer")]
    static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(LevelObjectPlacer));
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("LevelObject Placer", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        playerSpawnPrefab = EditorGUILayout.ObjectField("PlayerSpawn Prefab:", playerSpawnPrefab, typeof(GameObject), true) as GameObject;
        killDotPrefab = EditorGUILayout.ObjectField("KillDot Prefab:", killDotPrefab, typeof(GameObject), true) as GameObject;

        // get the grid reference and get a reference to all the griddots
        if (grid == null)
        {
            GameObject[] grids;
            grids = GameObject.FindGameObjectsWithTag("Grid");
            if (grids.Length > 0)
            {
                grid = grids[0];
                if (gridDots == null)
                {
                    Grid g = grid.GetComponent<Grid>();
                    gridDots = g.BuildDotArray();
                }
            }
        }

        
        objectType = (LevelObject.Type)EditorGUILayout.EnumPopup("Object to Place:", objectType);
        
        EditorGUILayout.Space();

        // the dot prefab was referenced
        if (grid)
        {
            switch (objectType)
            {
                case LevelObject.Type.Player:
                    if (playerSpawnPrefab)
                    {
                        //mode = (PlaceMode)EditorGUILayout.EnumPopup("Placement Mode:", mode);

                        if (GUILayout.Button("Place"))
                        {
                            
                        }
                    }
                    else
                    {
                        EditorGUILayout.LabelField("You haven't selected a PlayerSpawn Prefab!");
                    }
                    break;
                default:
                    break;
            }
        }
        else
        {
            EditorGUILayout.Space();
            GUI.contentColor = Color.red;
            EditorGUILayout.LabelField("The Grid GameObject couldn't be found. Is its Tag set to 'Grid'?");
        }
    }
}
