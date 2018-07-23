using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SoundDot : MonoBehaviour {
    

    public int repeatAmount = 1;
    public RythmManager.BPM repeatIntervall = RythmManager.BPM.bpm30;
    public bool waitForNextBPMtoPlay = false; // play on touch vs wait for bpm
    public Sound sound = null;

    private bool wasTouched = false;
    private int repeatedCount = 0;

    public void Start()
    {
        sound.source = gameObject.AddComponent<AudioSource>();
        sound.source.clip = sound.clip;
        sound.source.volume = sound.volume;
        sound.source.pitch = sound.pitch;
        sound.source.loop = sound.loop;

        RythmManager.onBPM.AddListener(OnBPM);
    }
	
    private void OnBPM(BPMinfo bpm)
    {
        if(Game.state == Game.State.Playing)
        {
            if (bpm.Equals(new BPMinfo(repeatIntervall)) && repeatAmount >= repeatedCount + 1)
            {
                if (IsTouchingPlayer(false))
                {
                    sound.source.Play();
                    repeatedCount++;
                }
            }
        }
    }

    /*
	// Update is called once per frame
	void Update () {
		if(Game.state == Game.State.Playing)
        {
            if (!wasTouched && IsTouchingPlayer(false) && sound != null)
            {
                if (!waitForNextBPMtoPlay)
                {
                    sound.source.Play();
                    repeatedCount++;
                }
                wasTouched = true;
            }
        }
	}*/

    // is a filled playerdot touching the object?
    public bool IsTouchingPlayer(bool playerDotNeedsToBeFilled)
    {
        if (playerDotNeedsToBeFilled)
        {
            if (((Player.dot0 != null && Player.dot0.transform.position == transform.position && Player.dot0.state == PlayerDot.State.Full) || (Player.dot1 != null && Player.dot1.transform.position == transform.position && Player.dot1.state == PlayerDot.State.Full)))
                return true;
        }
        else
            if (((Player.dot0 != null && Player.dot0.transform.position == transform.position) || (Player.dot1 != null && Player.dot1.transform.position == transform.position)))
            return true;
        return false;
    }


}
