using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterDot : LevelObject
{
    public static TeleporterDot instance;

    ParticleSystem particleFeedback;

	// Use this for initialization
	void Start ()
    {
        instance = this;
        particleFeedback = GetComponent<ParticleSystem>();
        RythmManager.onBPM.AddListener(OnRythmPlayAudio);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger");
        if (collision.gameObject.CompareTag("Player"))
        {          
            if (Teleporter.teleportEnabled)
            {
                Teleporter.teleporterTouched = true;
                Debug.Log("TELEPORTER TOUCHED" + Teleporter.teleporterTouched);

                if (Player.dot0.transform.position == this.transform.position)
                {
                    Teleporter.dot0OnTeleportPosition = true;
                }

                else
                {
                    Teleporter.dot0OnTeleportPosition = false;
                }
            }
            Debug.Log("PLAYER ON TELEPORTERDOT");
        }
    }

    void OnRythmPlayAudio(BPMinfo bpm)
    {
        if (Game.state == Game.State.Playing && (bpm.Equals(RythmManager.playerBPM) || bpm.Equals(RythmManager.playerDashBPM)))
        {
            if ((Player.dot0 != null && Player.dot0.transform.position == transform.position) || (Player.dot1 != null && Player.dot1.transform.position == transform.position))
            {
                AudioManager.instance.Play("Warp");
                particleFeedback.Play();
            }
        }
    }
}
