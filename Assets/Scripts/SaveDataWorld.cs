using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class SaveDataWorld
{
    // fill this with all the data we want to store
    public int strength;

    // character
    public int shard;
    public CharacterClassType classType;
    public int completedDialogue;

    // token
    public List<string> unlockedTokens = new List<string>();

    // world
    public int act;

    // town
    public List<BuildingType> unlockedBuildings = new List<BuildingType>();

    //progressions
    public string[] allClearedProgression;
    public string missionId;

    // stats tracker
    public List<BuildingType> buildingTrackerKey;
    public List<int> buildingTrackerValues;
    public List<CharacterClassType> classTypes;
    public List<int> level;
    public List<string> enemyId;
    public List<int> enemyAmountKilled;
    public List<CharacterClassType> unlockedCharacters = new List<CharacterClassType>();
    public int tokenPoints;
    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public void LoadFromJson(string a_Json)
    {
        JsonUtility.FromJsonOverwrite(a_Json, this);
    }
}

public interface ISaveableWorld
{
    void PopulateSaveDataWorld(SaveDataWorld a_SaveData);
    void LoadFromSaveDataWorld(SaveDataWorld a_SaveData);
}