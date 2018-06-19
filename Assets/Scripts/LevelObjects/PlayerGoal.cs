using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGoal : LevelObject
{
    public static PlayerGoal instance;

    ParticleSystem goalFeedback;
    public static bool respawnTimer = false;

    private void Start()
    {
        RythmManager.onBPM.AddListener(OnRythmCount);
        goalFeedback = GetComponent<ParticleSystem>();
    }

    /*
    private void Update()
    {
        Respawn();
    }
    */

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger");
        if (collision.gameObject.CompareTag("Player"))
        {
            respawnTimer = true;
            
        }
    }

    void OnRythmCount(BPMinfo bpm)
    {
        if(bpm.Equals(RythmManager.playerBPM))
        {

            if (respawnTimer == true)
            {
                respawnTimer = false;
                goalFeedback.Play();
                AudioManager.instance.Play("Goal");
                Game.SetState(Game.State.Death);
                Player.allowMove = false;

            }

            
        }
    }
 
    /*
    void Respawn()
    {
        if (beatCount == 1)
        {
            respawnTimer = false;
            beatCount = 0;

            Player.instance.Death();
        }
    }
    */
}
