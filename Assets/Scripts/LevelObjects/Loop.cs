using SubjectNerd.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loop : MonoBehaviour {

    public static int totalLoopsCounter = 0;

    [Reorderable]
    public List<LoopDot> loopDots = new List<LoopDot>();
    public List<LoopSegment> segments = new List<LoopSegment>();
    public int loopIndex = 0;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
