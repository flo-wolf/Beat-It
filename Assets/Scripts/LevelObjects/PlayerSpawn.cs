using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : LevelObject {

    private bool playerHasSpawned = false;
    ParticleSystem spawnFeedback;

	// Use this for initialization
	void Start ()
    {
        spawnFeedback = GetComponent<ParticleSystem>();
		// if the spawn is not attached to a griddot, attach it to a random one
        if(gridDot == null)
        {
            
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger");
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player Spawned");
            spawnFeedback.Play();
        }
    }
}
