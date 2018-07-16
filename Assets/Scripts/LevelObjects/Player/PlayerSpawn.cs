using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : LevelObject {

    public static PlayerSpawn instance = null;
    //private Animation anim = null;

    private bool playerHasSpawned = false;
    ParticleSystem spawnFeedback;

    private float defaultSize = 0f;

    public override void Awake()
    {
        // don't delete this, it overrides the levelobjects awake function which would otherwise set this levelobjects scale to 0.
    }

    // Use this for initialization
    void Start ()
    {
        instance = this;
        defaultSize = transform.localScale.x;

        transform.localScale = Vector3.zero;

        //anim = GetComponent<Animation>();
        EditPlayerSpawnData.ReportPlayerSpawnPosition(transform.position, Game.level);

        spawnFeedback = GetComponent<ParticleSystem>();
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
    
    public void Fade(float duration, bool fadeIn)
    {
        Debug.Log("FadeInPlayer Spawned");
        StartCoroutine(C_FadeSize(duration, fadeIn));
    }

    IEnumerator C_FadeSize(float duration, bool fadeIn)
    {
        float size = 0;

        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            if (fadeIn)
            {
                size = Mathfx.Berp(0, defaultSize, (elapsedTime / duration));
            }

            else
            {
                size = Mathfx.Berp(defaultSize, 0, (elapsedTime / duration));
            }
            transform.localScale = new Vector3(size, size, size);
            yield return null;
        }
        yield return null;
    }

}
