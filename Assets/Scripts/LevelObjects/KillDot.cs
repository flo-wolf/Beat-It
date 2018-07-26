using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillDot : LevelObject {

    public static bool jumpTouchedKilldot = false;

    [HideInInspector]
    public ParticleSystem killFeedback;

    private Animation deathAnim;

    bool kill = false;

    int count = 0;

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
        if (collision.gameObject.CompareTag("Player") && Game.state != Game.State.NextLevelFade)
        {
            if(count < 1)
            {
                AudioManager.instance.Play("OnDeathTrigger");
                killFeedback.Play();
                count++;
            }
            kill = true;
            jumpTouchedKilldot = true;
        }
    }
    

    public void OnGameStateChange(Game.State state)
    {
        if(state == Game.State.Playing && deathAnim != null)
        {
            deathAnim.clip.SampleAnimation(gameObject, 0);
        }
    }

    public void OnRythmCount(BPMinfo bpm)
    {
        if (bpm.Equals(RythmManager.playerBPM) || bpm.Equals(RythmManager.playerDashBPM))
        {
            if ((kill) && Game.state == Game.State.Playing)
            {
                Game.SetState(Game.State.DeathOnNextBeat);

                count = 0;
            }
            /*
            //Debug.Log(name);
            if ((Player.dot0 != null && Player.dot0.transform.position == transform.position) || (Player.dot1 != null && Player.dot1.transform.position == transform.position))
            {
                //Debug.Log("2 " + name );
                kill = false;
            }
            */
        }
    }
}
