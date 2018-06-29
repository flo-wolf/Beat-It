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

    [Reorderable]
    public List<LoopDot> loopDots = new List<LoopDot>();
    public List<LoopSegment> segments = new List<LoopSegment>();
    public int loopIndex = 0;

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
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
