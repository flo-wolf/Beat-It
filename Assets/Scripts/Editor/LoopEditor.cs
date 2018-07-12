using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LoopEditor : EditorWindow
{
    private Vector3 tempPos = Vector3.zero;

    // prefabs
    private GameObject containerGo = null;
    private GameObject loopPrefab = null;
    private GameObject segmentPrefab = null;
    private GameObject loopDotPrefab = null;

    // delete placed loop
    private GameObject loopGameObject = null;
    // delete placed loop dot
    private GameObject loopDotGameObject = null;

    private List<GridDot> gridDots = new List<GridDot>();

    private bool loopCreated = false;

    [MenuItem("Custom/Loop Editor")]
    static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(LoopEditor));
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Loop Editor", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Delete a Loop", EditorStyles.boldLabel);
        loopGameObject = EditorGUILayout.ObjectField("Loop in the Scene:", loopGameObject, typeof(GameObject), true) as GameObject;
        if (GUILayout.Button("Delete this Loop and its LoopDots"))
        {
            Loop l = loopGameObject.GetComponent<Loop>();
            if(l != null)
            {
                foreach(LoopDot d in l.loopDots)
                {
                    DestroyImmediate(d.gameObject);
                }
                DestroyImmediate(l.gameObject);
                l = null;
                loopGameObject = null;
            }
            loopCreated = false;
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Delete a Loop Dot", EditorStyles.boldLabel);
        loopDotGameObject = EditorGUILayout.ObjectField("LoopDot in the Scene:", loopDotGameObject, typeof(GameObject), true) as GameObject;
        if (GUILayout.Button("Delete this LoopDot and remove it from its Loops List"))
        {
            LoopDot l = loopDotGameObject.GetComponent<LoopDot>();
            l.loop.loopDots.Remove(l);
            DestroyImmediate(l.gameObject);
            loopCreated = false;
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Create a new Loop", EditorStyles.boldLabel);
        loopPrefab = EditorGUILayout.ObjectField("Loop Prefab:", loopPrefab, typeof(GameObject), true) as GameObject;
        loopDotPrefab = EditorGUILayout.ObjectField("LoopDot Prefab:", loopDotPrefab, typeof(GameObject), true) as GameObject;
        segmentPrefab = EditorGUILayout.ObjectField("LoopSegment Prefab:", segmentPrefab, typeof(GameObject), true) as GameObject;

        if (segmentPrefab != null && loopDotPrefab != null && loopPrefab != null)
        {
            EditorGUILayout.Space();
            string text = "To make a new Loop, first select the griddots you want to be part of the loop in the hierarchy.\nThen click the button.";
            EditorGUILayout.TextArea(text);

            if (GUILayout.Button("Create a new Loop"))
            {
                loopCreated = false;
                gridDots = new List<GridDot>();

                // find all the selected griddots in the hierarchy
                GameObject[] selectedObjects = Selection.gameObjects;

                bool proceed = true;
                if (selectedObjects.Length == 0)
                {
                    proceed = false;
                }
                Debug.Log(selectedObjects.Length);

                foreach (GameObject go in selectedObjects)
                {
                    GridDot dot = go.GetComponent<GridDot>();
                    if (dot == null && dot.gameObject.transform.childCount == 0 && dot.active)
                    {
                        proceed = false;
                        break;
                    }
                    else if (!gridDots.Contains(dot))
                        gridDots.Add(dot);
                }

                // lets proceed, there are only dots selected that are also free (no children)
                if (proceed && gridDots.Count > 0)
                {
                    
                    // find the non grid container to place the loop inside
                    if (containerGo == null)
                    {
                        GameObject[] containers;
                        containers = GameObject.FindGameObjectsWithTag("NonGridContainer");
                        if (containers.Length > 0)
                        {
                            containerGo = containers[0];
                        }
                    }

                    if(containerGo != null)
                    {
                        // place a loop GameObject inside the container
                        GameObject loopGo = (GameObject)PrefabUtility.InstantiatePrefab(loopPrefab);
                        //GameObject loopGo = (GameObject)Instantiate(loopPrefab, Vector3.zero, Quaternion.identity);
                        loopGo.transform.parent = containerGo.transform;
                        
                        // get the Loop component of the newly created loop gameobject
                        Loop loop = loopGo.GetComponent<Loop>();
                        Loop.totalLoopsCounter++;
                        loop.loopIndex = Loop.totalLoopsCounter;

                        loopGo.name = "Loop " + loop.loopIndex;

                        if (loop != null)
                        {
                            int dotCounter = 0;
                            // add loopdots to the griddots
                            foreach(GridDot gridDot in gridDots)
                            {
                                dotCounter++;
                                GameObject loopDotGo = (GameObject)PrefabUtility.InstantiatePrefab(loopDotPrefab);
                                loopDotGo.transform.parent = gridDot.transform;
                                loopDotGo.transform.localPosition = Vector3.zero;
                                loopDotGo.transform.localScale = loopDotPrefab.transform.localScale;
                                loopDotGo.name = "LoopDot " + loop.loopIndex + "-" + dotCounter;
                                LoopDot loopDot = loopDotGo.GetComponent<LoopDot>();
                                if(loopDot != null)
                                {
                                    loopDot.loop = loop;
                                    loop.loopDots.Add(loopDot);
                                }
                            }

                            // add a segment to the loop
                            GameObject segmentGo = (GameObject)PrefabUtility.InstantiatePrefab(segmentPrefab);
                            segmentGo.transform.parent = loopGo.transform;

                            // get the Segment component and add it to the loop segments list
                            LoopSegment segment = segmentGo.GetComponent<LoopSegment>();
                            segment.loop = loop;
                            loop.segments.Add(segment);
                            segmentGo.name = "LoopSegment " + loop.loopIndex + "-" + loop.segments.Count;

                            loopCreated = true;
                        }
                    }
                }
                else
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("You Havent selected any valid GridDots in the Hierarchy!", EditorStyles.boldLabel);
                    EditorGUILayout.Space();
                }
            }

            if (loopCreated)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Loop Created!", EditorStyles.boldLabel);
                EditorGUILayout.Space();
            }
        }
        else
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("You haven't selected the Prefabs yet!");
        }
    }
}
