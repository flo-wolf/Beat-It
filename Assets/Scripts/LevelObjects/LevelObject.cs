using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An Object that is tied to a certain GridDot and placed ontop
/// </summary>
public class LevelObject : MonoBehaviour {

    
    public enum Type { Unset, Player, LoopDot, Rotator }

    [Header("LevelObject Settings")]
    public Type type = Type.Unset;

    // the gridDot this object is tied to (aka placed ontop)
    public GridDot gridDot;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
