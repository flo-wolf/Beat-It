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
    public GridDot[] gridDots;
    int lenghtOfList;

    //The StartDot where the MovingKD was placed
    GridDot startDot;

    //The direction the MovingKD is looking at
    Vector2 lookDirection;

    bool onStart;
    bool onEnd;

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

        lenghtOfList = gridDots.Length;
    }

    
    void OnRythmMove(BPMinfo bpm)
    {
        if(bpm.Equals(RythmManager.movingKillDotBPM))
        {
            UpdateLookDirection();
            Move();
        }
    }

    void UpdateLookDirection()
    {
        if (transform.position == startDot.transform.position)
        {
            lookDirection = gridDots[0].transform.position - transform.position;
            lookDirection = lookDirection.normalized;
            onStart = true;
        }

        if (transform.position == gridDots[lenghtOfList-1].transform.position)
        {
            lookDirection = startDot.transform.position - transform.position;
            lookDirection = lookDirection.normalized;
            onEnd = true;
        }

        else
        {
            foreach(GridDot dot in gridDots)
            {
                if(!onStart && !onEnd)
                {
                    if(this.transform.position == dot.transform.position)
                    {
                        int positionInList = Array.IndexOf(gridDots, dot);
                        lookDirection = gridDots[positionInList + 1].transform.position - dot.transform.position;
                    }
                }
            }
        }
    }
    
    void Move()
    {
        Remove();

        GridDot activeDot = transform.parent.gameObject.GetComponent<GridDot>();
        GridDot nextDot = Grid.GetNearestActiveMovingDot(activeDot, lookDirection);

        this.transform.parent = nextDot.transform;
        this.transform.localPosition = Vector3.zero;

        Appear();
        onStart = false;
        onEnd = false;
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
        /*
        if (!fadeIn)
        {
            gridDot.levelObject = null;
            //GameObject.Destroy(gameObject);
        }
        */
        yield return null;
    }
}
