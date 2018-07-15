using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerSpawnData : ScriptableObject
{
    public Vector3[] positions;

    void ForceSerialization()
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    public void SetPosition(Vector3 position, int index)
    {
        positions[index] = position;
        ForceSerialization();
    }
}
