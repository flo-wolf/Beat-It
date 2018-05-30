using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RythmClock : MonoBehaviour {

    public LineRenderer lineRenderer;
    public float radius = 3f;

    private float turnDegree = 0f;

	void Start () {
        
    }
	
	void Update ()
    {

        float tick = (1 / RythmManager.instance.clockBPM) * 60;
        // calculate the current turning degree of the clock pointer
        turnDegree = RythmManager.clock.Remap(0f, tick, 2f * Mathf.PI + Mathf.PI/2, Mathf.PI / 2);

        DrawPointer();
    }

    void DrawPointer()
    {
        if(lineRenderer != null)
        {
            lineRenderer.positionCount = 2;

            Vector2 lineStart = Vector2.zero;

            float endX = (radius * Mathf.Cos(turnDegree));
            float endY = (radius * Mathf.Sin(turnDegree));
            Vector2 lineEnd = new Vector2(endX, endY);

            lineRenderer.SetPosition(0, lineStart);
            lineRenderer.SetPosition(1, lineEnd);
        }
    }
}
