using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGoal : LevelObject
{
    public static PlayerGoal instance;

    ParticleSystem goalFeedback;

    //public static bool respawn = false;

    private void Start()
    {
        RythmManager.onBPM.AddListener(OnRythmRespawn);
        goalFeedback = GetComponent<ParticleSystem>();
    }

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger");
        if (collision.gameObject.CompareTag("Player"))
        {
            Player.allowMove = false;
        }
    }
   
    void OnRythmRespawn(BPMinfo bpm)
    {
        if (bpm.Equals(RythmManager.playerBPM))
        {
            if(Player.dot0 != null && Player.dot0.transform.position == gameObject.transform.parent.position)
            {
                goalFeedback.Play();
                AudioManager.instance.Play("Plop");

                Game.SetState(Game.State.Death);
            }

            else if (Player.dot1 != null && Player.dot1.transform.position == gameObject.transform.parent.position)
            {
                goalFeedback.Play();
                AudioManager.instance.Play("Plop");

                Game.SetState(Game.State.Death);
            }
        }
    }
}
