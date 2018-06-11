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
}
