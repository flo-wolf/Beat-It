using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Drawing class for segments 
 * Segments = Lines that move from point A to B, like the Moving parts of the loop or the player connection
 * */
public class Segment : MonoBehaviour {


    [Header("Settings")]
    public float fillTime = 0.5f;
    public float emptyTime = 0.5f;

    [Header("Components")]
    public LineRenderer lineRenderer;

    // Segment drawing information provided by the Player Class on PlayerDot creation
    public Vector2 startPoint = new Vector2();
    public Vector2 endPoint = new Vector2();

    public float fillProgress = 0f;

    // the state of this segment
    public enum State { NoDraw, Filling, Filled, Emptying, Shooting };
    public State state = State.NoDraw;

    public void FillSegment(Vector2 start, Vector2 end)
    {
        state = State.Filling;
        startPoint = start;
        endPoint = end;
        fillProgress = 0;
        StopCoroutine("EmptyCoroutine");
        StartCoroutine(FillCoroutine());
    }

    public void EmptySegment(bool switchDirection)
    {
        Debug.Log("EmptySegment " + PlayerSegment.touchedKillDot);
        if (PlayerSegment.touchedKillDot)
        {
            PlayerSegment.instance.AdaptKillColor();
        }

        if (state == State.Filled || state == State.Filling)
        {
            state = State.Emptying;

            if (switchDirection)
            {
                Vector2 memory = startPoint;
                startPoint = endPoint;
                endPoint = memory;
            }

            StopCoroutine("FillCoroutine");
            StartCoroutine(EmptyCoroutine());
        }
    }

    // Loop Segment shooting
    public void ShootSegment(Vector2 start, Vector2 end, float length, float duration)
    {
        state = State.Shooting;
        startPoint = start;
        endPoint = end;
        fillProgress = 0;
        StopCoroutine("ShootCoroutine");
        StartCoroutine(ShootCoroutine(length, duration));
    }


    IEnumerator ShootCoroutine(float length, float duration)
    {
        float elapsedTime = 0f;
        float startFill = fillProgress;

        while (elapsedTime <= duration)
        {
            fillProgress = Mathf.SmoothStep(startFill, 1, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return null;
    }

    // interpolates the progress of the segment i order to fill it
    IEnumerator FillCoroutine()
    {
        float elapsedTime = 0f;
        float startFill = fillProgress;

        while (elapsedTime <= fillTime && state == State.Filling)
        {
            fillProgress = Mathf.SmoothStep(startFill, 1, (elapsedTime / fillTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        if (state == State.Filling)
            state = State.Filled;

        yield return null;
    }

    // interpolates the progress of the segment in order to empty it
    IEnumerator EmptyCoroutine()
    {
        float elapsedTime = 0f;
        float startFill = fillProgress;

        while (elapsedTime <= emptyTime && state == State.Emptying)
        {
            fillProgress = Mathf.SmoothStep(startFill, 0, (elapsedTime / emptyTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        if (state == State.Emptying)
            state = State.NoDraw;

        yield return null;
    }
}
