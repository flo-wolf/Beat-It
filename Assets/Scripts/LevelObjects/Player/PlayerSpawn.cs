using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : LevelObject {

    private bool playerHasSpawned = false;
    ParticleSystem spawnFeedback;

	// Use this for initialization
	void Start ()
    {
        EditPlayerSpawnData.ReportPlayerSpawnPosition(transform.position, Game.level);

        spawnFeedback = GetComponent<ParticleSystem>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Trigger");
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player Spawned");
            //spawnFeedback.Play();
        }
    }
}
