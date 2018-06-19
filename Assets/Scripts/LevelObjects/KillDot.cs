using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillDot : LevelObject {

    ParticleSystem killFeedback;

    bool kill = false;

    private void Start()
    {
        killFeedback = GetComponent<ParticleSystem>();
        RythmManager.onBPM.AddListener(OnRythmCount);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger");
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerSegment.touchedKillDot = true;
            //PlayerSegment.instance.AdaptKillColor();
            kill = true;
            Player.allowMove = false;
        }
    }

    void OnRythmCount(BPMinfo bpm)
    {
        if (bpm.Equals(RythmManager.playerBPM))
        {
            if (kill)
            {
                killFeedback.Play();
                AudioManager.instance.Play("Death");
                AudioManager.instance.Play("Slurp");
                Game.SetState(Game.State.Death);
                
                Debug.Log("Kill the player");
                kill = false;
            }
        }
    }
}
