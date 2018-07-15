using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleScript : MonoBehaviour {

    ParticleSystem ps;

	// Use this for initialization
	void Start ()
    {
        RythmManager.onBPM.AddListener(OnRythmPlay);
        ps = this.GetComponent<ParticleSystem>();
	}

    void OnRythmPlay(BPMinfo bpm)
    {
        if (bpm.Equals(RythmManager.animationBPM))
        {
            if (Player.allowMove)
            {
                ps.Play();
            }
        }
    }
}
