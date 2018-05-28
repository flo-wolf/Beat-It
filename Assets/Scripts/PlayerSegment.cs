using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSegment : MonoBehaviour {

    [Header("Settings")]
    public float fillTime = 0.5f;
    public float emptyTime = 0.5f;

    [Header("Components")]
    public LineRenderer lineRenderer;

    // Segment drawing information provided by the Player Class on PlayerDot creation
    private Vector2 startPoint = new Vector2();
    private Vector2 endPoint = new Vector2();
    public static float fillProgress = 0f;

    // the state of this segment
    public enum State { NoDraw, Filling, Filled, Emptying };
    private State state = State.NoDraw;

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

            AudioManager.instance.Play("Snare");
        }
    }

    // interpolates the progress of the segment i order to fill it
    IEnumerator FillCoroutine()
    {
        float elapsedTime = 0f;
        float startFill = fillProgress;

        while (elapsedTime <= fillTime && state == State.Filling)
        {
            fillProgress = Mathf.Lerp(startFill, 1, (elapsedTime / fillTime));
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
            fillProgress = Mathf.Lerp(startFill, 0, (elapsedTime / emptyTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        if (state == State.Emptying)
            state = State.NoDraw;

        yield return null;
    }

    // Update is called once per frame
    void Update ()
    {
		if(state != State.NoDraw)
        {
            lineRenderer.positionCount = 2;
            Vector2 toEnd = endPoint - startPoint;

            Vector2 lineStart = startPoint;
            Vector2 lineEnd = Vector2.Lerp(startPoint, endPoint, fillProgress);

            lineRenderer.SetPosition(0, lineEnd);
            lineRenderer.SetPosition(1, lineStart);
        }
        else
        {
            lineRenderer.positionCount = 0;
        }
	}

    // Helper Function
}
