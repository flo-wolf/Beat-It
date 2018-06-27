using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterDot : LevelObject
{
    public static TeleporterDot instance;

    ParticleSystem particleFeedback;

    public static bool teleporterTouched;
    public static bool teleportEnabled = true;

	// Use this for initialization
	void Start ()
    {
        instance = this;
        particleFeedback = GetComponent<ParticleSystem>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger");
        if (collision.gameObject.CompareTag("Player"))
        {
            if(teleportEnabled)
            {
                teleporterTouched = true;
            }

            particleFeedback.Play();

        }
    }
}
