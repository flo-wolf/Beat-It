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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger");
        if (collision.gameObject.CompareTag("Player"))
        {
            respawnTimer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Trigger");
        if (collision.gameObject.CompareTag("Player"))
        {
            respawnTimer = false;
        }
    }

    void OnRythmCount(BPMinfo bpm)
    {
        if (bpm.Equals(RythmManager.playerBPM) && Player.instance.CheckIfSingleDot())
        {

            if (respawnTimer == true)
            {
                Player.allowMove = false;
                respawnTimer = false;

                goalFeedback.Play();
                AudioManager.instance.Play("Plop");

                Game.SetState(Game.State.Death);
            }
        }
    }
}
