using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Moves the two background layers of stars
/// </summary>
public class BackgroundAnimator : MonoBehaviour {

    public static BackgroundAnimator instance;

    [Header("Components")]
    public SpriteRenderer farSr;
    public SpriteRenderer closeSr;

    // duration
    [Header("Durations")]
    public float moveDurationFar = .5f;
    public float moveDutationFarVariation = .1f;
    public float moveDurationClose = .5f;
    public float moveDutationCloseVariation = .1f;

    // distances
    [Header("Distances")]
    public float moveDistanceFar = 2f;
    public float moveDistatanceFarVariation = .5f;
    public float moveDistanceClose = 2f;
    public float moveDistatanceCloseVariation = .5f;

    // origins
    private Vector3 originFar;
    private Vector3 originClose;


    void Start () {

        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(this.gameObject);

        DontDestroyOnLoad(this);

        originFar = farSr.transform.position;
        originClose = closeSr.transform.position;

        MoveFar();
        MoveClose();
    }

    public void MoveFar()
    {
        StopCoroutine("C_MoveFar");
        StartCoroutine(C_MoveFar(farSr.transform.position, GetNextPoint(true), GetNextDuration(true)));
    }

    public void MoveClose()
    {
        StopCoroutine("C_MoveClose");
        StartCoroutine(C_MoveClose(closeSr.transform.position, GetNextPoint(false), GetNextDuration(false)));
    }

    IEnumerator C_MoveFar(Vector3 start, Vector3 end, float duration, bool recursive = true)
    {
        Vector3 newPosition = new Vector3();
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            newPosition = Vector3.Lerp(start, end, Mathf.Clamp((elapsedTime/duration), 0, 1));
            farSr.transform.position = newPosition;
            yield return null;
        }

        if(recursive)
            MoveFar();
        yield return null;
    }

    IEnumerator C_MoveClose(Vector3 start, Vector3 end, float duration, bool recursive = true)
    {
        Vector3 newPosition = new Vector3();
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            newPosition = Vector3.Lerp(start, end, Mathf.Clamp((elapsedTime / duration), 0, 1));
            closeSr.transform.position = newPosition;
            yield return null;
        }

        if (recursive)
            MoveClose();
        yield return null;
    }

    public Vector3 GetNextPoint(bool far)
    {
        Vector3 current = new Vector3();
        Vector3 next = new Vector3();
        if (far)
        {
            current = originFar;

            float variation = 0f;
            float plusMinus = Random.Range(-1, 1);
            if (plusMinus < 0)
                variation = moveDistatanceFarVariation / 2;
            else
                variation = (-moveDistatanceFarVariation) / 2;

            next = new Vector3(current.x + Random.Range(((-moveDistanceFar)/2) + variation, moveDistanceFar/2 + variation), current.y + Random.Range(((-moveDistanceFar) / 2) + variation, moveDistanceFar/2 + variation), current.z);

        }
        else
        {
            current = originClose;

            float variation = 0f;
            float plusMinus = Random.Range(-1, 1);
            if (plusMinus < 0)
                variation = moveDistatanceCloseVariation/2;
            else
                variation = (-moveDistatanceCloseVariation)/2;

            next = new Vector3(current.x + Random.Range(((-moveDistanceClose) / 2) + variation, moveDistanceClose/2 + variation), current.y + Random.Range(((-moveDistanceClose) / 2) + variation, moveDistanceClose/2 + variation), current.z);
        }
        return next;
    }

    public float GetNextDuration(bool far)
    {
        float next = 0f;
        if (far)
        {
            next = Random.Range(moveDurationFar - moveDutationFarVariation, moveDurationFar + moveDutationFarVariation);
        }
        else
        {
            next = Random.Range(moveDurationClose - moveDutationCloseVariation, moveDurationClose + moveDutationCloseVariation);
        }
        return next;
    }


    /*
    public void GameStateChanged(Game.State state)
    {
        if (state == Game.State.Death)
        {
            StopCoroutine("C_MoveFar");
            StopCoroutine("C_MoveClose");

            float duration = Game.levelSwitchDuration / 2;
            StartCoroutine(C_MoveFar(farSr.transform.position, originFar, duration, false));
            StartCoroutine(C_MoveClose(closeSr.transform.position, originClose, duration, false));
        }
    }
    */
}
