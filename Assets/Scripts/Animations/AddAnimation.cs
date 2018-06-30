using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddAnimation : MonoBehaviour
{

    Animation anim;
    AnimationCurve curve;
    AnimationClip clip;

    // Use this for initialization
    void Start()
    {
        // move the player on the beat
        RythmManager.onBPM.AddListener(OnRythmAnimate);
        AddAnimationToObject();
    }

    void AddAnimationToObject()
    {
        anim = gameObject.AddComponent<Animation>();
        clip = new AnimationClip();
        clip.legacy = true;
        //If you want to loop the animation
        //clip.wrapMode = WrapMode.Loop;

        Keyframe[] keys;
        keys = new Keyframe[3];
        keys[0] = new Keyframe(AnimationManager.instance.start, AnimationManager.instance.startValue);
        keys[1] = new Keyframe(AnimationManager.instance.mid, AnimationManager.instance.midValue);
        keys[2] = new Keyframe(AnimationManager.instance.end, AnimationManager.instance.endValue);

        //Debug.Log(keys);

        curve = new AnimationCurve(keys);

        clip.SetCurve("", typeof(Transform), "localScale.x", curve);
        clip.SetCurve("", typeof(Transform), "localScale.y", curve);
        clip.SetCurve("", typeof(Transform), "localScale.z", curve);
    }

    void PlayAnimation()
    {
        anim.AddClip(clip, "Scale");
        anim.Play("Scale");
    }

    void OnRythmAnimate(BPMinfo bpm)
    {
        if (bpm.Equals(RythmManager.animationBPM))
        {
            PlayAnimation();
        }
    }
}
