using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMovingKillDot : LevelObject
{
    [Header("Settings")]
    public float fadeDuration = 0.5f;

    [Header("Components")]
    public SpriteRenderer sr;

    [Header("List of GridDots")]
    //List of GridDots to define where the Dot should move
    public List<GridDot> gridDotList;

    int lenghtOfList;
    int listPosition;

    //The StartDot where the MovingKD was placed
    GridDot startDot;

    //The direction the MovingKD is looking at
    Vector2 lookDirection;

    private float defaultLocalScale;            // default player dot size
    private float scale = 0f;
    private float opacity = 0f;

    ParticleSystem killFeedback;

    private Animation deathAnim;

    private void Start()
    {
        RythmManager.onBPM.AddListener(OnRythmMove);

        defaultLocalScale = transform.localScale.x;
        StartCoroutine(Fade(true));

        killFeedback = GetComponent<ParticleSystem>();

        deathAnim = GetComponentInChildren<Animation>();

        startDot = transform.parent.gameObject.GetComponent<GridDot>();
        gridDotList.Insert(0, startDot);
        listPosition = 0;

        lenghtOfList = gridDotList.Count;
    }

    void OnRythmMove(BPMinfo bpm)
    {
        if(bpm.Equals(RythmManager.movingKillDotBPM))
        {
            UpdateLookDirection();
            MoveToNextDot();
        }
    }

    void UpdateLookDirection()
    {
        foreach(GridDot dot in gridDotList)
        {
            if(this.transform.position == gridDotList[listPosition].transform.position)
            {
                if(listPosition < (lenghtOfList-1))
                {
                    lookDirection = gridDotList[listPosition + 1].transform.position - this.transform.position;
                    lookDirection = lookDirection.normalized;
                    listPosition = listPosition + 1;
                }

                else if(listPosition == (lenghtOfList-1))
                {
                    lookDirection = gridDotList[0].transform.position - this.transform.position;
                    lookDirection = lookDirection.normalized;
                    listPosition = 0;
                }

                Debug.Log("LIST POSITION" + listPosition);
            }
        }
    }
    
    void MoveToNextDot()
    {
        Remove();

        GridDot activeDot = transform.parent.gameObject.GetComponent<GridDot>();
        GridDot nextDot = Grid.GetNearestActiveMovingDot(activeDot, lookDirection);

        this.transform.parent = nextDot.transform;
        this.transform.localPosition = Vector3.zero;

        Appear();
    }

    public void Remove()
    {
        StopCoroutine("Fade");
        StartCoroutine(Fade(false));
    }
    
    public void Appear()
    {
        StopCoroutine("Fade");
        StartCoroutine(Fade(true));
    }

    public void PlayParticleSystem()
    {
        killFeedback.Play();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (deathAnim != null)
                deathAnim.Play("MovingKillDotDeathFast");
        }
    }

    IEnumerator Fade(bool fadeIn)
    {
        float elapsedTime = 0f;
        float startScale = scale;
        float startOpacity = opacity;

        while (elapsedTime <= fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            Color c = sr.color;
            if (fadeIn)
            {
                scale = Mathf.SmoothStep(startScale, defaultLocalScale, (elapsedTime / fadeDuration));
                transform.localScale = new Vector3(scale, scale, 1);

                opacity = Mathf.SmoothStep(startOpacity, 1, (elapsedTime / fadeDuration));
            }
            else
            {
                scale = Mathf.SmoothStep(startScale, 0, (elapsedTime / fadeDuration));
                transform.localScale = new Vector3(scale, scale, 1);
                opacity = Mathf.SmoothStep(startOpacity, 0, (elapsedTime / fadeDuration));
            }

            c.a = opacity;
            sr.color = c;

            yield return null;
        }

        yield return null;
    }
}
