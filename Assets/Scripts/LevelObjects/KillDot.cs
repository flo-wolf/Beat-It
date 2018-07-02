using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillDot : LevelObject {

    [HideInInspector]
    public ParticleSystem killFeedback;

    private Animation deathAnim;

    bool kill = false;

    private void Start()
    {
        Game.onGameStateChange.AddListener(OnGameStateChange);
        deathAnim = GetComponent<Animation>();
        killFeedback = GetComponent<ParticleSystem>();
        RythmManager.onBPM.AddListener(OnRythmCount);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger");
        if (collision.gameObject.CompareTag("Player"))
        {
            kill = true;
            PlayerSegment.touchedKillDot = true;
            Player.allowMove = false;
            Game.SetState(Game.State.Death);

            deathAnim.Play();
            killFeedback.Play();
        }
    }

    public void OnGameStateChange(Game.State state)
    {
        if(state == Game.State.Playing)
        {
            deathAnim.clip.SampleAnimation(gameObject, 0);
        }
    }

    public void OnRythmCount(BPMinfo bpm)
    {
        if (bpm.Equals(RythmManager.playerBPM) || bpm.Equals(RythmManager.playerDashBPM))
        {
            //Debug.Log(name);
            if ((Player.dot0 != null && Player.dot0.transform.position == transform.position) || (Player.dot1 != null && Player.dot1.transform.position == transform.position))
            {
                //Debug.Log("2 " + name );
                AudioManager.instance.Play("Death");
                kill = false;
            }
        }
    }
}
