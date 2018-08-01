using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CreateAssetMenu]
public static class CreatePlayerSpawnDataMenuItem
{
#if UNITY_EDITOR

    [MenuItem("Custom/Level Management/Create New PlayerSpawnData Holder")]
    public static void CreateGameLevelHolder()
    {
        PlayerSpawnData playerSpawnDataHolder = ScriptableObject.CreateInstance<PlayerSpawnData>();
        playerSpawnDataHolder.positions = new Vector3[10];

        AssetDatabase.CreateAsset(playerSpawnDataHolder,
         "Assets/Data/PlayerSpawnDataHolder.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = playerSpawnDataHolder;
    }
#endif
}
