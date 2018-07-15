using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopSegment : Segment {

    public float segmentLength = 5f;
    public bool segmentLengthPercent = false;

    public Loop loop = null;
    public int currentLoopIndex= 0;
    public bool moveUpLoopList = true; // 0,1,2,3 if true, 0,3,2,1 if false

    private EdgeCollider2D colliderEdge;

    private LoopDot nextDot = null;

    // In case loop.seperateShotSuck == true: was the segment shot out? if so, suck it in.
    private bool segmentShot = false;

    private Color lineRendererColor;



    // Use this for initialization
    void Start ()
    {
        colliderEdge = GetComponent<EdgeCollider2D>();
        RythmManager.onBPM.AddListener(OnBPM);
        Game.onGameStateChange.AddListener(GameStateChanged);

        lineRendererColor = lineRenderer.startColor;

    }

    private void OnBPM(BPMinfo bpm)
    {
        if (Game.state == Game.State.Playing)
        {
            if ( (bpm.Equals(RythmManager.playerBPM) && !loop.offBeat)
                || (bpm.Equals(BPMinfo.ToHalf(RythmManager.playerBPM)) && loop.offBeat))
            {
                if((loop.delayOneBeat && !loop.beatDelayed) || !loop.delayOneBeat)
                {
                    if ((loop.skipEverySecondBeat && !loop.skippedThisBeat) || (loop.seperateShootSuck && segmentShot))
                    {
                        if (nextDot != null)
                        {
                            nextDot.SegmentRecieved();
                            AudioManager.instance.Play("HiHat");
                        }
                        MoveToNextLoopDot();
                    }
                    loop.skippedThisBeat = !loop.skippedThisBeat;
                    loop.beatDelayed = false;
                }
                loop.beatDelayed = !loop.beatDelayed;
            }
        }
    }

    private void MoveToNextLoopDot()
    {
        if (!loop.seperateShootSuck)
        {
            LoopDot currentDot = loop.loopDots[currentLoopIndex];

            nextDot = null;
            if (moveUpLoopList)
                currentLoopIndex = loop.GetNextLoopDot(currentLoopIndex);
            else
                currentLoopIndex = loop.GetPrevLoopDot(currentLoopIndex);
            nextDot = loop.loopDots[currentLoopIndex];

            ShootSegment(currentDot.gridDot, nextDot.gridDot, segmentLength, RythmManager.playerBPM.ToSecs() / 2);
        }
        else
        {
            //ShootSegment(currentDot.gridDot, nextDot.gridDot, segmentLength, RythmManager.playerBPM.ToSecs() * 2);
            if (!segmentShot)
            {
                LoopDot currentDot = loop.loopDots[currentLoopIndex];

                nextDot = null;
                if (moveUpLoopList)
                    currentLoopIndex = loop.GetNextLoopDot(currentLoopIndex);
                else
                    currentLoopIndex = loop.GetPrevLoopDot(currentLoopIndex);
                nextDot = loop.loopDots[currentLoopIndex];

                FillSegment(currentDot.gridDot, nextDot.gridDot, RythmManager.playerBPM.ToSecs());
                segmentShot = true;
            }
            else
            {
                EmptySegment(true, RythmManager.playerBPM.ToSecs());
                segmentShot = false;
            } 
        }
            
    }

    IEnumerator ShootCoroutine(Vector3 start, Vector3 end, float duration)
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

    // Update the endpoints of the lienrenderer to fit the calculated lineEnd and lineStart points
    void Update()
    {
        if (state != State.NoDraw)
        {
            Vector2 lineStart = new Vector2();
            Vector2 lineEnd = new Vector2();
            lineRenderer.positionCount = 2;

            // hoot a segment with a fixed length
            if (!loop.seperateShootSuck)
            {
                float fillProgressSegment = 0f;

                if (!segmentLengthPercent)
                {
                    fillProgressSegment = segmentLength / (endDot.transform.position - startDot.transform.position).magnitude;
                }
                else
                {
                    fillProgressSegment = (endDot.transform.position - startDot.transform.position).magnitude * segmentLength;
                }

                lineStart = Vector2.Lerp(startDot.transform.position, endDot.transform.position, Mathf.Clamp(fillProgress - (fillProgressSegment - (fillProgressSegment * fillProgress)), 0, 1));
                lineEnd = Vector2.Lerp(startDot.transform.position, endDot.transform.position, Mathf.Clamp(fillProgress, 0, 1));
            }

            // shoot a segment that fills the whole area between the start and enddot, then let it suck back in
            else
            {
                lineStart = startDot.transform.position;
                lineEnd = Vector2.Lerp(startDot.transform.position, endDot.transform.position, fillProgress);
            }

            lineRenderer.SetPosition(0, lineEnd);
            lineRenderer.SetPosition(1, lineStart);

            Vector2[] colliderpoints;
            colliderpoints = colliderEdge.points;
            colliderpoints[0] = lineEnd;
            colliderpoints[1] = lineStart;
            colliderEdge.points = colliderpoints;
        }
        else
        {
            lineRenderer.positionCount = 0;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("------- LoopSegment touched by " + collision.name);
        if (collision.gameObject.CompareTag("Player") && Game.state == Game.State.Playing)
        {

            if (!PlayerIsOnLoopDot())
            {
                Debug.Log("------- LoopSegment touched by player");
                PlayerSegment.touchedKillDot = true;
                Player.allowMove = false;
                //Player.deathBySegment = true;
                Game.SetState(Game.State.Death);
            }
        }
    }

    public bool PlayerIsOnLoopDot()
    {
        foreach(LoopDot dot in loop.loopDots)
        {
            if (dot.IsTouchingPlayer(false))
                return true;
        }
        return false;
    }

    void GameStateChanged(Game.State state)
    {
        switch (state)
        {
            case Game.State.Death:
                StartCoroutine(C_FadeOutSegment(RythmManager.playerBPM.ToSecs()/2));
                break;
            case Game.State.NextLevelFade:
                StartCoroutine(C_FadeOutSegment(RythmManager.playerBPM.ToSecs()/2));
                break;
            case Game.State.Playing:
                lineRenderer.startColor = lineRendererColor;
                lineRenderer.endColor = lineRendererColor;
                break;
        }
    }
}
