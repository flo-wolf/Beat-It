using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingKillDotHandler : MonoBehaviour {

    public static MovingKillDotHandler instance;

    //This GameObject will be instantiated as the new Dot at the new GridDot position.
    public GameObject movingKillDotPrefab;

    //Particle System
    public ParticleSystem killFeedback;
    public ParticleSystem oldFeedback;

    //This will reference the latest dot
    MovingKillDot newDot = null;
    //This will reference the dot before
    MovingKillDot oldDot = null;

    //Define where the MovingKillDot should start
    public GridDot startDot;
    //Define where the MovingKillDot should end
    public GridDot endDot;

    GridDot parentDot;

    //The initial spawn position for the MovingKillDot
    Vector2 spawnPosition;

    //Will be calculated by the start and end
    Vector2 lookDirection;

    bool forward = false;
    bool reverse = false;

    public static bool kill = false;

    [HideInInspector]
    public static bool allowMove = false;

    // Use this for initialization
    void Start ()
    {
        instance = this;

        RythmManager.onBPM.AddListener(OnRythmMove);

        lookDirection = endDot.transform.position - startDot.transform.position;
        lookDirection = lookDirection.normalized;

        //Debug.Log("MovingKillDot Direction: " + lookDirection);

        StartSpawn();
    }

    private void Update()
    {
        UpdateLookBool();
        UpdateLookDirection();
        CheckPlayerDotPosition();
    }


    void OnRythmMove(BPMinfo bpm)
    {
        if(bpm.Equals(RythmManager.movingKillDotBPM))
        {
            if(allowMove)
            {
                SpawnMovingKillDot();
                AudioManager.instance.Play("MovingKillDotSnare");

                if (newDot != null)
                {
                    DestroyOldMovingKillDot();
                }
            }
        }

        if(bpm.Equals(RythmManager.playerBPM))
        {
            if (kill)
            {
                PlayerSegment.touchedKillDot = true;
                Player.allowMove = false;

                if(Player.dot0 != null && Player.dot1 !=null)
                {
                    if(Player.dot1.transform.position == newDot.transform.position)
                    {
                        Player.instance.RemoveDot(true);
                    }

                    else if(Player.dot0.transform.position == newDot.transform.position)
                    {
                        Player.instance.RemoveDot(false);
                    }
                }

                Game.SetState(Game.State.Death);

                Instantiate(killFeedback, newDot.transform.position, Quaternion.identity);
                AudioManager.instance.Play("Death");

                kill = false;
            }
        }
    }

    void CheckPlayerDotPosition()
    {
        if((Player.dot0 != null && Player.dot0.transform.position == newDot.transform.position) || (Player.dot1 != null && Player.dot1.transform.position == newDot.transform.position))
        {
            kill = true;
            allowMove = false;
            Player.allowMove = false;
        }
    }

    void UpdateLookDirection()
    {
        if(forward)
        {
            lookDirection = endDot.transform.position - newDot.transform.position;
            lookDirection = lookDirection.normalized;
        }

        else if(reverse)
        {
            lookDirection = startDot.transform.position - newDot.transform.position;
            lookDirection = lookDirection.normalized;
        }
    }

    void UpdateLookBool()
    {
        if (newDot.transform.position == startDot.transform.position)
        {
            Debug.Log("REACHED STARTDOT");
            forward = true;
            reverse = false;
        }

        if(newDot.transform.position == endDot.transform.position)
        {
            Debug.Log("REACHED ENDDOT");
            reverse = true;
            forward = false;
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
        parentDot = Grid.GetNearestActiveMovingDot(newDot.gridDot, lookDirection);
        oldDot = newDot;
        //Debug.Log("ParentDot MovingKillDot: " + parentdot.transform.position);

        //spawn the new MovingKillDot
        GameObject newDotToGo = GameObject.Instantiate(movingKillDotPrefab, Vector2.zero, Quaternion.identity);
        newDotToGo.transform.parent = parentDot.transform;
        newDotToGo.transform.localPosition = Vector3.zero;
        newDotToGo.transform.localScale = movingKillDotPrefab.transform.localScale;

        MovingKillDot newMovingKillDot = newDotToGo.GetComponent<MovingKillDot>();
        parentDot.levelObject = newMovingKillDot;

        newDot = newMovingKillDot;

        Instantiate(oldFeedback, newDot.transform.position, Quaternion.identity);

        //Debug.Log("NewDot: " + newDot.transform.position);
    }

    void DestroyOldMovingKillDot()
    {
        oldDot.Remove();
    }
}
