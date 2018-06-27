using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopDot : KillDot {

    public Loop loop = null;

    private void Start()
    {
        killFeedback = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update () {
	}
}
