using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingKillDotHandler : MonoBehaviour {

    public static MovingKillDotHandler instance;

    //This GameObject will be instantiated as the new Dot at the new GridDot position.
    public GameObject movingKillDotPrefab;

    //This will reference the latest dot
    MovingKillDot newDot = null;
    //MovingKillDot oldDot = null;

    //Define where the MovingKillDot should start
    public GridDot startDot;
    //Define where the MovingKillDot should end
    public GridDot endDot;

    //The initial spawn position for the MovingKillDot
    Vector2 spawnPosition;

    //Will be calculated by the start and end
    Vector2 lookDirection;

    //Our initial starting Direction;
    Vector2 startDirection;
    Vector2 reverseDirection;

    [HideInInspector]
    public bool destroy = false;

    // Use this for initialization
    void Start ()
    {
        instance = this;

        RythmManager.onBPM.AddListener(OnRythmMove);

        lookDirection = endDot.transform.position - startDot.transform.position;
        lookDirection = lookDirection.normalized;

        startDirection = lookDirection;
        reverseDirection = lookDirection * -1;
        Debug.Log(reverseDirection);

        //Debug.Log("MovingKillDot Direction: " + lookDirection);

        StartSpawn();
    }

    private void Update()
    {
        UpdateLookDirection();
    }


    void OnRythmMove(BPMinfo bpm)
    {
        if(bpm.Equals(RythmManager.playerBPM))
        {
            SpawnMovingKillDot();
        }
    }
    
    void UpdateLookDirection()
    {
        if(newDot.transform.position == startDot.transform.position)
        {
            Debug.Log("REACHED STARTDOT");
            lookDirection = startDirection;
        }

        if(newDot.transform.position == endDot.transform.position)
        {
            Debug.Log("REACHED ENDDOT");
            lookDirection = reverseDirection;
        }
    }

    void StartSpawn()
    {
        spawnPosition = startDot.transform.position;
        GameObject firstDotToGo = GameObject.Instantiate(movingKillDotPrefab, Vector2.zero, Quaternion.identity);
        firstDotToGo.transform.parent = startDot.transform;
        firstDotToGo.transform.localPosition = Vector3.zero;
        firstDotToGo.transform.localScale = movingKillDotPrefab.transform.localScale;

        MovingKillDot firstMovingKillDot = firstDotToGo.GetComponent<MovingKillDot>();
        startDot.levelObject = firstMovingKillDot;

        newDot = firstMovingKillDot;
    }

    void SpawnMovingKillDot()
    {
        GridDot parentdot = Grid.GetNearestActiveDot(newDot.gridDot, lookDirection);
        //Debug.Log("ParentDot MovingKillDot: " + parentdot.transform.position);

        //spawn the new MovingKillDot
        GameObject newDotToGo = GameObject.Instantiate(movingKillDotPrefab, Vector2.zero, Quaternion.identity);
        newDotToGo.transform.parent = parentdot.transform;
        newDotToGo.transform.localPosition = Vector3.zero;
        newDotToGo.transform.localScale = movingKillDotPrefab.transform.localScale;

        MovingKillDot newMovingKillDot = newDotToGo.GetComponent<MovingKillDot>();
        parentdot.levelObject = newMovingKillDot;

        newDot = newMovingKillDot;
        //Debug.Log("NewDot: " + newDot.transform.position);

        destroy = true;
    }
}
