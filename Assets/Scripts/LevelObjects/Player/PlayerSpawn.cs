using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : LevelObject {

    public static PlayerSpawn instance = null;
    //private Animation anim = null;

    private bool playerHasSpawned = false;
    ParticleSystem spawnFeedback;


    public override void Awake()
    {
        // don't delete this, it overrides the levelobjects awake function which would otherwise set this levelobjects scale to 0.
    }

    // Use this for initialization
    void Start ()
    {
        instance = this;

        //anim = GetComponent<Animation>();
        EditPlayerSpawnData.ReportPlayerSpawnPosition(transform.position, Game.level);

        LevelTransition.onLevelTransition.AddListener(OnLevelTransition);

        spawnFeedback = GetComponent<ParticleSystem>();
    }

    void OnLevelTransition(LevelTransition.Action action)
    {
        switch (action)
        {
            case LevelTransition.Action.SpawnFadeIn:
                Fade(true);
                break;
            case LevelTransition.Action.SpawnFadeOut:
                Fade(false);
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Trigger");
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player Spawned");
            //spawnFeedback.Play();
        }
    }
    
    private void Fade(bool fadeIn)
    {
        if (fadeIn)
        {

            StartCoroutine(C_FadeSize(RythmManager.playerBPM.ToSecs() / 2, true));
            //anim["Spawn_FadeIn"].speed = 1;
            //anim["Spawn_FadeIn"].time = 0;
            //anim.Play("Spawn_FadeIn");
        }
        else
        {
            StartCoroutine(C_FadeSize(RythmManager.playerBPM.ToSecs() / 2, false));
            //anim["Spawn_FadeIn"].speed = -1;
            //anim["Spawn_FadeIn"].time = anim["Spawn_FadeIn"].length;
            //anim.Play("Spawn_FadeIn");
        }
    }

    IEnumerator C_FadeSize(float duration, bool fadeIn)
    {
        float startSize = transform.localScale.x;
        float size = 0;

        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            if (fadeIn)
            {
                size = Mathf.SmoothStep(0, startSize, (elapsedTime / duration));
            }

            else
            {
                size = Mathf.SmoothStep(startSize, 0, (elapsedTime / duration));
            }
            transform.localScale = new Vector3(size, size, size);
            yield return null;
        }
        yield return null;
    }

}
