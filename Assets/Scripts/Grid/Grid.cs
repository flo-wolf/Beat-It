using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public static Grid instance;
    public static GridDot[,] gridDots;

    public int rows = -1; // gets set by the GridPlacer Editor Window when modifying the grid

    public int columns = -1; // gets set by the GridPlacer Editor Window when modifying the grid

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

    // smoothly fade out all the griddots in an expanding circle around the startdot
    public static void FadeGridDotsGradually(Vector3 center, float duration, float radius, bool fadeIn, Action callback = null)
    {
        instance.StartCoroutine(instance.C_FadeGridDotsGradually(center, duration, radius, fadeIn, callback));
    }

    IEnumerator C_FadeGridDotsGradually(Vector3 center, float duration, float endRadius, bool fadeIn, Action callback = null)
    {
        float elapsedTime = 0f;
        float radius = 0;

        // dots that have been found and faded already
        List<GameObject> detectedDots = new List<GameObject>();

        Debug.Log("fade grid dots in gradually");
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            if(LevelTransition.instance.smoothDotFadeLerp)
                radius = Mathf.SmoothStep(0, endRadius, (elapsedTime / duration));
            else
                radius = Mathf.Lerp(0, endRadius, (elapsedTime / duration));

            Collider2D[] colliders = Physics2D.OverlapCircleAll(center, radius, 1 << LayerMask.NameToLayer("GridDot"));
            foreach(Collider2D collider in colliders)
            {
                //Debug.Log("found a collider: -"+collider.gameObject.tag+"-");
                if (collider.gameObject.CompareTag("GridDot") && !detectedDots.Contains(collider.gameObject))
                {
                    //Debug.Log("found a griddot");
                    detectedDots.Add(collider.gameObject);
                    GridDot gridDot = collider.GetComponent<GridDot>();
                    if(gridDot != null)
                    {
                        //Debug.Log("Found a girddot to fade out!");
                        gridDot.LevelTransitionFade(fadeIn);
                        yield return null;
                    }
                }
            }

            yield return null;
        }
        
        if (callback != null)
        {
            yield return new WaitForSeconds(RythmManager.playerBPM.ToSecs());
            callback();
        }
            
        yield return null;
    }


    // smoothly fade out all the griddots in an expanding circle around the startdot
    public static void FadeLevelObjectsGradually(Vector3 center, float duration, float radius, bool fadeIn, Action callback = null)
    {
        instance.StartCoroutine(instance.C_FadeLevelObjectsGradually(center, duration, radius, fadeIn, callback));
    }


    public IEnumerator C_FadeLevelObjectsGradually(Vector3 center, float duration, float endRadius, bool fadeIn, Action callback = null)
    {
        float elapsedTime = 0f;
        float radius = 0;

        // dots that have been found and faded already
        List<GameObject> detectedObjects = new List<GameObject>();

        Debug.Log("fade level objects in gradually");
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            if (LevelTransition.instance.smoothDotFadeLerp)
                radius = Mathf.SmoothStep(0, endRadius, (elapsedTime / duration));
            else
                radius = Mathf.Lerp(0, endRadius, (elapsedTime / duration));

            Collider2D[] colliders = Physics2D.OverlapCircleAll(center, radius, 1 << LayerMask.NameToLayer("LevelObject"));
            foreach (Collider2D collider in colliders)
            {
                //Debug.Log("found a collider: -"+collider.gameObject.tag+"-");
                if (!detectedObjects.Contains(collider.gameObject))
                {
                    //Debug.Log("found a griddot");
                    detectedObjects.Add(collider.gameObject);
                    LevelObject obj = collider.GetComponent<LevelObject>();
                    if (obj != null)
                    {
                        Debug.Log("Found a girddot to fade out: " + obj.name);
                        obj.FadeIn(RythmManager.playerBPM.ToSecs()/2);
                        yield return null;
                    }
                }
            }

            yield return null;
        }

        if (callback != null)
        {
            yield return new WaitForSeconds(RythmManager.playerBPM.ToSecs());
            callback();
        }

        yield return null;
    }

    public static GridDot FindPlayerSpawn()
    {
        PlayerSpawn s = null;
        if (instance != null)
        {
            s = instance.GetComponentInChildren<PlayerSpawn>();
        }
        
        if(s != null)
        {
            GridDot parentDot = s.transform.parent.GetComponent<GridDot>();
            return parentDot;
        }
        return null;
    }

    public static GridDot FindPlayerGoal()
    {
        PlayerGoal g = instance.GetComponentInChildren<PlayerGoal>();
        if (g != null)
        {
            GridDot parentDot = g.transform.parent.GetComponent<GridDot>();
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
            g = gridDots[UnityEngine.Random.Range(0, instance.rows - 1), UnityEngine.Random.Range(0, instance.columns - 1)];
            if (g != null && g.active && g.levelObject == null && g.gameObject.activeSelf)
                return g;
        }
        return g;
    }

    // gather all six surrounding dots in CLOCKWISE ORDER, beginning top left.
    public static List<GridDot> GetSurroundingDots(GridDot dot)
    {
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

            // right dot
            if (dot.column + 1 < instance.columns)
                hexDots.Add(gridDots[dot.row, dot.column + 1]);

            // bot right dot
            if (dot.row + 1 < instance.rows && dot.column + 1 < instance.columns)
                hexDots.Add(gridDots[dot.row + 1, dot.column + 1]);

            // bot left dot
            if (dot.row + 1 < instance.rows)
                hexDots.Add(gridDots[dot.row + 1, dot.column]);

            // left dot
            if (dot.column - 1 >= 0)
                hexDots.Add(gridDots[dot.row, dot.column - 1]);
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
            // right dot
            if (dot.column + 1 < instance.columns)
                hexDots.Add(gridDots[dot.row, dot.column + 1]);
            // bot right dot
            if (dot.row + 1 < instance.rows)
                hexDots.Add(gridDots[dot.row + 1, dot.column]);
            // bot left dot
            if (dot.row + 1 < instance.rows && dot.column - 1 >= 0)
                hexDots.Add(gridDots[dot.row + 1, dot.column - 1]);
            // left dot
            if (dot.column - 1 >= 0)
                hexDots.Add(gridDots[dot.row, dot.column - 1]);
        }

        return hexDots;
    }

    // get the nearest dot based on a normalized input (thumbstick to the right => closest active dot on the right)
	public static GridDot GetNearestActiveDot(GridDot dot, Vector2 direction, bool jump = false)
    {
        Vector2 originalDirection = direction;
        // gather all six sourrounding dots
        List<GridDot> hexDots = new List<GridDot>();

        if (jump)
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
            if (d != null && d.active && d.gameObject.activeSelf && d.PlayerCanMoveOnto()) 
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
        if (closestDot == null && jump)
            return GetNearestActiveDot(dot, originalDirection, false);
        return closestDot;
    }

    public static GridDot GetNearestActiveMovingDot(GridDot dot, Vector2 direction)
    {

        // gather all six sourrounding dots
        List<GridDot> hexDots = new List<GridDot>();

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
                if ((dot.row % 2 == 0 && d.row % 2 != 0) || (dot.row % 2 != 0 && d.row % 2 == 0))
                {
                    float hypothenuse = Mathf.Sqrt(((instance.gridStep / 2) * (instance.gridStep / 2)) + ((instance.gridStep) * (instance.gridStep)));
                    float multi = instance.gridStep / hypothenuse;
                    l = l * multi;

                }

                if (l < closestLength)
                {
                    closestLength = l;
                    closestDot = d;
                }
            }
        }

        return closestDot;
    }


    public static void Pan(Vector3 start, Vector3 end, float duration, bool smoothStep = true)
    {
        instance.StartCoroutine(instance.C_Pan(start, end, duration));
    }

    IEnumerator C_Pan(Vector3 start, Vector3 end, float duration, bool smoothStep = true)
    {
        Vector3 gridOrigin = instance.transform.position;
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float extraLerp = Mathf.SmoothStep(0, 1, (elapsedTime / duration));
            instance.transform.position = Vector3.Slerp(gridOrigin, end-start, extraLerp);
            yield return null;
        }
        yield return null;
    }
}
