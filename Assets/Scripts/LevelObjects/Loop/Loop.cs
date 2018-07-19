using SubjectNerd.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loop : MonoBehaviour {

    public static int totalLoopsCounter = 0;

    public bool offBeat = false; 
    public bool skipEverySecondBeat = true;
    [HideInInspector]
    public bool skippedThisBeat = false;

    public bool seperateShootSuck = false;


    public bool delayOneBeat = false;
    [HideInInspector]
    public bool beatDelayed = true;

    [Reorderable]
    public List<LoopDot> loopDots = new List<LoopDot>();
    public List<LoopSegment> segments = new List<LoopSegment>();
    public int loopIndex = 0;

    public float directionIndicatorSpeed = 5f;
    int lenghtOfList;
    int indexOfDot;
    Transform target;

    public int GetPrevLoopDot(int currentIndex)
    {
        if (currentIndex - 1 < 0)
            return loopDots.Count - 1;
        else
            return currentIndex - 1;
    }

    public int GetNextLoopDot(int currentIndex)
    {
        if (currentIndex + 1 > loopDots.Count - 1)
            return 0;
        else
            return currentIndex + 1;
    }

    // Use this for initialization
    void Start ()
    {
        lenghtOfList = loopDots.Count;
	}
	
	// Update is called once per frame
	void Update ()
    {
        RotateLoopDotDirectionIndicator();
	}

    void RotateLoopDotDirectionIndicator()
    {
        foreach(LoopDot dot in loopDots)
        {
            indexOfDot = loopDots.IndexOf(dot);

            if(indexOfDot < lenghtOfList -1)
            {
                target = loopDots[indexOfDot + 1].transform;
            }

            else if(indexOfDot == lenghtOfList - 1)
            {
                target = loopDots[0].transform;
            }

            Vector2 direction = target.position - dot.transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            dot.transform.rotation = Quaternion.Slerp(dot.transform.rotation, rotation, directionIndicatorSpeed * Time.deltaTime);
        }
    }
}
