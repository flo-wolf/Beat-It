using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridDot : MonoBehaviour
{

    public Material materialActive;
    public Material materialDeactive;

    public int row = 0;
    public int column = 0;

    public bool active = true;

    // the object occupying our dot (player or levelobject)
    public LevelObject levelObj;

    void Start()
    {
        AdjustMaterial();
        AddAnimation();
    }

    void OnValidate()
    {
        AdjustMaterial();
    }

    void AdjustMaterial()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && materialActive != null && materialDeactive != null)
        {
            if (active)
                sr.material = materialActive;
            else
                sr.material = materialDeactive;
        }
    }

    void AddAnimation()
    {
        Animation anim = gameObject.AddComponent<Animation>();
        AnimationCurve curve;

        AnimationClip clip = new AnimationClip();
        clip.legacy = true;
        clip.wrapMode = WrapMode.Loop;

        Keyframe[] keys;
        keys = new Keyframe[3];
        keys[0] = new Keyframe(AnimationManager.instance.start, AnimationManager.instance.startValue);
        keys[1] = new Keyframe(AnimationManager.instance.mid, AnimationManager.instance.midValue);
        keys[2] = new Keyframe(AnimationManager.instance.end, AnimationManager.instance.endValue);

        Debug.Log(keys);

        curve = new AnimationCurve(keys);

        clip.SetCurve("", typeof(Transform), "localScale.x", curve);
        clip.SetCurve("", typeof(Transform), "localScale.y", curve);
        clip.SetCurve("", typeof(Transform), "localScale.z", curve);

        anim.AddClip(clip, "Scale");
        anim.Play("Scale");
    }
}
