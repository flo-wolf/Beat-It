using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGoal : LevelObject
{
    public static PlayerGoal instance;

    public Player player;

    ParticleSystem goalFeedback;

    public static bool respawnRemoveDot;

    int count = 0;

    //public static bool respawn = false;

    private void Start()
    {
        RythmManager.onBPM.AddListener(OnRythmRespawn);
        goalFeedback = GetComponent<ParticleSystem>();

        player = FindObjectOfType<Player>();
    }

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger");
        if (collision.gameObject.CompareTag("Player"))
        {
            if(count < 1)
            {
                AudioManager.instance.Play("ReachedGoal");
                goalFeedback.Play();

                count++;
            }

            Player.allowMove = false;
            respawnRemoveDot = true;
        }
    }
   
    void OnRythmRespawn(BPMinfo bpm)
    {
        if (bpm.Equals(RythmManager.playerBPM) || bpm.Equals(RythmManager.playerDashBPM))
        {
            if (!respawnRemoveDot)
            {
                if (Player.dot0 != null && Player.dot0.transform.position == gameObject.transform.parent.position)
                {
                    AudioManager.instance.Play("Goal");

                    Game.SetState(Game.State.Death);
                    count = 0;
                }

                else if (Player.dot1 != null && Player.dot1.transform.position == gameObject.transform.parent.position)
                {
                    AudioManager.instance.Play("Goal");

                    Game.SetState(Game.State.Death);
                    count = 0;
                }
            }

            else if(respawnRemoveDot)
            {
                if (Player.dot0 != null && Player.dot0.transform.position == gameObject.transform.parent.position)
                {
                    player.RemoveDot(false);
                    respawnRemoveDot = false;
                }

                else if (Player.dot1 != null && Player.dot1.transform.position == gameObject.transform.parent.position)
                {
                    player.RemoveDot(true);
                    respawnRemoveDot = false;
                }
            }
        }
    }
}
