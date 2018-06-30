using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopSegment : Segment {

    public float segmentLength = 5f;
    public bool segmentLengthPercent = false;

    public Loop loop = null;
    public int currentLoopIndex= 0;
    public bool moveUpLoopList = true; // 0,1,2,3 if true, 0,3,2,1 if false

    private EdgeCollider2D collider;

    private LoopDot nextDot = null;

	// Use this for initialization
	void Start ()
    {
        collider = GetComponent<EdgeCollider2D>();
        RythmManager.onBPM.AddListener(OnBPM);
	}

    private void OnBPM(BPMinfo bpm)
    {
        if (Game.state == Game.State.Playing)
        {
            if ( (bpm.Equals(RythmManager.playerBPM) && !loop.offBeat)
                || (bpm.Equals(BPMinfo.ToHalf(RythmManager.playerBPM)) && loop.offBeat))
            {
                if (loop.skipEverySecondBeat && !loop.skippedThisBeat)
                {
                    if (nextDot != null)
                    {
                        nextDot.SegmentRecieved();
                        AudioManager.instance.Play("HiHat");
                    }
                    MoveToNextLoopDot();
                }
                loop.skippedThisBeat = !loop.skippedThisBeat;
            }
        }
    }

    private void MoveToNextLoopDot()
    {
        LoopDot currentDot = loop.loopDots[currentLoopIndex];

        nextDot = null;
        if (moveUpLoopList)
            currentLoopIndex = loop.GetNextLoopDot(currentLoopIndex);
        else
            currentLoopIndex = loop.GetPrevLoopDot(currentLoopIndex);

        nextDot = loop.loopDots[currentLoopIndex];
        ShootSegment(currentDot.gridDot, nextDot.gridDot, segmentLength, RythmManager.playerBPM.ToSecs());
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
            lineRenderer.positionCount = 2;

            float fillProgressSegment = 0f;

            if (!segmentLengthPercent)
            {
                fillProgressSegment = segmentLength / (endDot.transform.position - startDot.transform.position).magnitude;
            }
            else
            {
                fillProgressSegment = (endDot.transform.position - startDot.transform.position).magnitude * segmentLength;
            }
            
            //Debug.Log("fillProgressSegment " + fillProgressSegment);

            Vector2 lineStart = Vector2.Lerp(startDot.transform.position, endDot.transform.position, Mathf.Clamp(fillProgress-(fillProgressSegment-(fillProgressSegment*fillProgress)), 0 , 1));
            Vector2 lineEnd = Vector2.Lerp(startDot.transform.position, endDot.transform.position, Mathf.Clamp(fillProgress, 0, 1));

            lineRenderer.SetPosition(0, lineEnd);
            lineRenderer.SetPosition(1, lineStart);

            Vector2[] colliderpoints;
            colliderpoints = collider.points;
            colliderpoints[0] = lineEnd;
            colliderpoints[1] = lineStart;
            collider.points = colliderpoints;

            
        }
        else
        {
            lineRenderer.positionCount = 0;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Touched Segment");
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerSegment.touchedKillDot = true;
            Player.allowMove = false;
            Game.SetState(Game.State.Death);
        }
    }
}
