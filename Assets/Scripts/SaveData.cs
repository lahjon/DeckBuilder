using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{   
    // fill this with all the data we want to store
    public int strength;

    // character
    public int shard;

    // world
    public int act;

    // town
    public List<BuildingType> unlockedBuildings = new List<BuildingType>();


    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public void LoadFromJson(string a_Json)
    {
        JsonUtility.FromJsonOverwrite(a_Json, this);
    }
}

public interface ISaveable
{
    void PopulateSaveData(SaveData a_SaveData);
    void LoadFromSaveData(SaveData a_SaveData);
}