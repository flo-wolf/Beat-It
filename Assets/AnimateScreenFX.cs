using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VortexStudios.PostProcessing;

public class AnimateScreenFX : MonoBehaviour {

    OLDTVFilter3 filter = null;
    public float offsetChangePerUpdate = 0.0001f;

    OLDTVPreset memoryPreset = null;

    // Use this for initialization
    void Start () {
        filter = GetComponent<OLDTVFilter3>();
        filter.preset.staticFilter.staticOffset = 0;
    }
	
	// Update is called once per frame
	void Update () {
        filter.preset.staticFilter.staticOffset += offsetChangePerUpdate;
    }
}
