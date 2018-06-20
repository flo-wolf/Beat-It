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
            Player.allowMove = false;
            Game.SetState(Game.State.Death);

            killFeedback.Play();
        }
    }

    void OnRythmCount(BPMinfo bpm)
    {
        if (bpm.Equals(RythmManager.playerBPM))
        {
            if (kill || (Player.dot0 != null && Player.dot0.transform.position == transform.position) || (Player.dot1 != null && Player.dot1.transform.position == transform.position))
            {
                AudioManager.instance.Play("Death");
                kill = false;
            }
        }
    }
}
