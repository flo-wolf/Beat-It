using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalDot : MonoBehaviour
{
    public static GoalDot instance;

    ParticleSystem goalFeedback;
    public static bool respawnTimer = false;

    int beatCount = 0;

    private void Start()
    {
        RythmManager.onBPM.AddListener(OnRythmCount);
        RythmManager.onBPM.AddListener(OnRythmRespawn);
        goalFeedback = GetComponent<ParticleSystem>();
    }

    /*
    private void Update()
    {
        Respawn();
    }
    */

    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("Trigger");
        if (collision.gameObject.CompareTag("Player"))
        {
            if(Player.instance.CheckDot())
            {
                respawnTimer = true;
            }
        }
    }

    void OnRythmCount(BPMinfo bpm)
    {
        if(bpm.Equals(RythmManager.playerBPM))
        {
            if(respawnTimer == true)
            {
                beatCount++;

                goalFeedback.Play();
                AudioManager.instance.Play("Goal");
            }
        }
    }

 
    void OnRythmRespawn(BPMinfo bpm)
    {
        if (bpm.Equals(RythmManager.playerBPM))
        {
            if (beatCount == 1)
            {
                respawnTimer = false;
                beatCount = 0;

                Player.instance.Death();
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
