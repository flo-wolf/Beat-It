using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// info class storing bpm event information
public class BPMinfo{

    public RythmManager.BPM bpm;
    [HideInInspector]
    public int bpmValue;

    public BPMinfo(RythmManager.BPM _bpm)
    {
        bpm = _bpm;
        bpmValue = RythmManager.BPMtoInt(_bpm);
    }

    public static BPMinfo ToHalf(BPMinfo bpmInfo)
    {
        RythmManager.BPM BPMh = RythmManager.BPMtoBPMh(bpmInfo.bpm);
        return new BPMinfo(BPMh);
    }

    // convert a bpm type to bpm halfs
    public float ToSecs()
    {
        return (1f / bpmValue) * 60;
    }

    public bool Equals(BPMinfo bpmInfo)
    {
        if (bpm == bpmInfo.bpm)
            return true;
        return false;
    }
}
