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
        //Debug.Log(data + " pos: " + position +  " level: " + level);
        if (data != null)
            data.SetPosition(position, level);
    }

    public static Vector3 GetPlayerSpawnPosition(int level)
    {
        PlayerSpawnData data = ReadPlayerSpawnData();
        if (data != null)
            return data.positions[level];
        else return Vector3.zero;
    }

    public static Vector3 GetNextPlayerSpawnPositon()
    {

        PlayerSpawnData data = ReadPlayerSpawnData();
        if (data != null)
        {
            int currentLevel = Game.level;
            if (Game.instance.levels.Count > Game.level + 1)
                return GetPlayerSpawnPosition(Game.level + 1);
            else
                return GetPlayerSpawnPosition(Game.level);
        } 
        else return Vector3.zero;
    }


    
}
