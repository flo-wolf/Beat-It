using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSegment : Segment {

    public static PlayerSegment instance;

    public Material killMaterial;

    private Color defaultColor;
    private LineRenderer lr;
    private Color killColor;
    private PolygonCollider2D collider;

    public static bool touchedKillDot = false;

    private void Start()
    {
        instance = this;
        Game.onGameStateChange.AddListener(GameStateChanged);

        collider = GetComponent<PolygonCollider2D>();
        collider.pathCount = 1;

        lr = GetComponent<LineRenderer>();
        if (lr != null)
        {
            defaultColor = lr.startColor;
            killColor = killMaterial.color;

            lr.startColor = defaultColor;
            lr.endColor = defaultColor;
        }
    }

    private void GameStateChanged(Game.State state)
    {
        switch (state)
        {
            case Game.State.Playing:
                touchedKillDot = false;
                lr.endColor = defaultColor;
                lr.startColor = defaultColor;
                break;
            case Game.State.DeathOnNextBeat:
                AdaptKillColor();
                break;
            case Game.State.Death:
                StartCoroutine(C_FadeOutSegment(RythmManager.playerBPM.ToSecs() / 2));
                break;
            case Game.State.NextLevelFade:
                StartCoroutine(C_FadeOutSegment(RythmManager.playerBPM.ToSecs() / 2));
                break;
        }
    }

    public void AdaptKillColor()
    {
        Debug.Log("--------ADAPT KILL COLOR--------");
        touchedKillDot = true;
        lr.endColor = killColor;
        lr.startColor = killColor;
        touchedKillDot = false;
    }

    // Update the endpoints of the lienrenderer to fit the calculated lineEnd and lineStart points
    void Update()
    {
        if (state != State.NoDraw)
        {
            lineRenderer.positionCount = 2;

            Vector2 lineStart = startDot.transform.position;
            Vector2 lineEnd = Vector2.Lerp(startDot.transform.position, endDot.transform.position, fillProgress);

            lineRenderer.SetPosition(0, lineEnd);
            lineRenderer.SetPosition(1, lineStart);

            Vector2[] colliderpoints = new Vector2[4];
            colliderpoints[0] = lineEnd;
            colliderpoints[1] = lineStart;
            colliderpoints[2] = new Vector3(lineEnd.x + 0.01f, lineEnd.y + 0.01f, 0);
            colliderpoints[3] = new Vector3(lineStart.x + 0.01f, lineStart.y + 0.01f, 0);
            collider.points = colliderpoints;

        }
        else
        {
            lineRenderer.positionCount = 0;
        }
    }

}
