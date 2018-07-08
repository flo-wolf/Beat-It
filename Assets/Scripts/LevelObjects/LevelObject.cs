using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An Object that is tied to a certain GridDot and placed ontop
/// </summary>
public class LevelObject : MonoBehaviour {

    
    public enum Type { Unset, Player, LoopDot, KillDot, MovingKillDot, Rotator, PlayerSpawn, PlayerGoal, TeleporterDot}

    [Header("LevelObject Settings")]
    public Type type = Type.Unset;

    public bool playerCanMoveOnto = true;

    // the gridDot this object is tied to (aka placed ontop)
    [HideInInspector]
    public GridDot gridDot;

    // is a filled playerdot touching the object?
    public bool IsTouchingPlayer(bool playerDotNeedsToBeFilled)
    {
        if (playerDotNeedsToBeFilled)
        {
            if (type != Type.Player && ((Player.dot0 != null && Player.dot0.transform.position == transform.position && Player.dot0.state == PlayerDot.State.Full) || (Player.dot1 != null && Player.dot1.transform.position == transform.position && Player.dot1.state == PlayerDot.State.Full)))
                return true;
        }
        else
            if (type != Type.Player && ((Player.dot0 != null && Player.dot0.transform.position == transform.position) || (Player.dot1 != null && Player.dot1.transform.position == transform.position)))
            return true;
        return false;
    }
}
