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
    public GridDot startDot = null;
    public GridDot endDot = null;

    public float fillProgress = 0f;

    // the state of this segment
    public enum State { NoDraw, Filling, Filled, Emptying, Shooting };
    public State state = State.NoDraw;

    public void FillSegment(GridDot start, GridDot end, float duration)
    {
        state = State.Filling;
        startDot = start;
        endDot = end;
        fillProgress = 0;
        StopCoroutine("EmptyCoroutine");
        StartCoroutine(FillCoroutine(duration));
    }

    public void EmptySegment(bool switchDirection, float duration)
    {
        //Debug.Log("EmptySegment " + PlayerSegment.touchedKillDot);
        if (PlayerSegment.touchedKillDot)
        {
            PlayerSegment.instance.AdaptKillColor();
        }

        if (state == State.Filled || state == State.Filling)
        {
            state = State.Emptying;

            if (switchDirection)
            {
                GridDot memory = startDot;
                startDot = endDot;
                endDot = memory;
            }

            StopCoroutine("FillCoroutine");
            StartCoroutine(EmptyCoroutine(duration));
        }
    }

    // Loop Segment shooting
    public void ShootSegment(GridDot start, GridDot end, float length, float duration)
    {
        state = State.Shooting;
        startDot = start;
        endDot = end;
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
    IEnumerator FillCoroutine(float duration)
    {
        float elapsedTime = 0f;
        float startFill = fillProgress;

        while (elapsedTime <= duration && state == State.Filling)
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
    IEnumerator EmptyCoroutine(float duration)
    {
        float elapsedTime = 0f;
        float startFill = fillProgress;

        while (elapsedTime <= duration && state == State.Emptying)
        {
            fillProgress = Mathf.SmoothStep(startFill, 0, (elapsedTime / emptyTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        if (state == State.Emptying)
            state = State.NoDraw;

        yield return null;
    }

    public IEnumerator C_FadeOutSegment(float duration)
    {
        float elapsedTime = 0f;
        Color startColor = lineRenderer.startColor;
        startColor.a = 1;
        Color endColor = Camera.main.backgroundColor;
        endColor.a = 1;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, (elapsedTime / duration));
            Color currentColor = lineRenderer.startColor;
            Color lerpColor = Color.Lerp(currentColor, endColor, (elapsedTime / duration));

            //lerpColor.a = alpha;


            lineRenderer.startColor = lerpColor;
            lineRenderer.endColor = lerpColor;
            yield return null;
        }

        lineRenderer.startColor = endColor;
        lineRenderer.endColor = endColor;
        yield return null;
    }
}
