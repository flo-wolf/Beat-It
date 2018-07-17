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

    [HideInInspector]
    public Vector3 originalScale = new Vector3();

    // the gridDot this object is tied to (aka placed ontop)
    [HideInInspector]
    public GridDot gridDot;

    public virtual void Awake()
    {
        originalScale = transform.localScale;
        transform.localScale = Vector3.zero;
    }

    public void FadeIn(float duration)
    {
        StartCoroutine(C_Fade(true, duration));
    }

    public void FadeOut(float duration)
    {
        StartCoroutine(C_Fade(false, duration));
    }

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

    IEnumerator C_Fade(bool fadeIn, float duration)
    {
        Debug.Log("PlayerDot fadeIn: " + fadeIn + " -- name: " + gameObject.name);
        float elapsedTime = 0f;
        Vector3 size = Vector3.zero;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            size = Vector3.Lerp(Vector3.zero, originalScale, (elapsedTime / duration));
            transform.localScale = size;
            yield return null;
        }

        transform.localScale = originalScale;
        yield return null;
    }
}
