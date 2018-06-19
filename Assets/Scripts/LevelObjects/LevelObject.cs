using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An Object that is tied to a certain GridDot and placed ontop
/// </summary>
public class LevelObject : MonoBehaviour {

    
    public enum Type { Unset, Player, LoopDot, KillDot, Rotator, PlayerSpawn, PlayerGoal }

    [Header("LevelObject Settings")]
    public Type type = Type.Unset;

    // the gridDot this object is tied to (aka placed ontop)
    [HideInInspector]
    public GridDot gridDot;
}
