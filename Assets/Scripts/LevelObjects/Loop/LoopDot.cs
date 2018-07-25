using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopDot : KillDot {

    public Loop loop = null;
    private Animation anim;

    private void Start()
    {
        anim = GetComponent<Animation>();
        killFeedback = GetComponent<ParticleSystem>();
        RythmManager.onBPM.AddListener(OnRythmCount);
    }

    // a segment has touched this dot => grow animation
    public void SegmentRecieved()
    {
        anim.Play();
    }
}
