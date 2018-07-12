using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EditPlayerSpawnData
{
    private const string _path = "Data/";
    private const string _defaultName = "PlayerSpawnDataHolder";

    public static PlayerSpawnData ReadPlayerSpawnData()
    {
        object o = Resources.Load(_path + _defaultName);
        PlayerSpawnData retrievedPlayerSpawnData = (PlayerSpawnData)o;
        return retrievedPlayerSpawnData;
    }

    public static void ReportPlayerSpawnPosition(Vector3 position, int level)
    {
        PlayerSpawnData data = ReadPlayerSpawnData();
        Debug.Log(data + " pos: " + position +  " level: " + level);
        if (data != null)
            data.positions[level] = position;
    }
}
