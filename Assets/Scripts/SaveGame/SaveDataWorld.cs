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
    public List<CardWrapper> unlockedCards;
    public int idxCounter;

    // token
    public List<int> unlockedTokens;

    // world
    public List<int> availableWorldEncounters;
    public List<int> completedWorldEncounters;

    // town
    public List<BuildingType> unlockedBuildings;

    // perk
    public List<int> unlockedPerks = new List<int>();

    //progressions
    public List<int> clearedObjectives;
    public List<int> currentObjectives;
    public List<IntListWrapper> currentObjectiveGoals;

    public List<int> clearedMissions;
    public List<int> currentMissions;
    public List<IntListWrapper> currentMissionGoals;

    // stats tracker
    public List<BuildingType> buildingTrackerKey;
    public List<int> buildingTrackerValues;
    public List<CharacterClassType> classTypes;
    public List<int> level;
    public List<int> enemyId;
    public List<int> enemyAmountKilled;
    public List<CharacterClassType> unlockedCharacters = new List<CharacterClassType>();
    public List<Profession> unlockedProfessions = new List<Profession>();
    
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

[System.Serializable]
public class ListWrapper<T>
{
    public List<T> aList = new List<T>();
}

[System.Serializable]
public class IntListWrapper : ListWrapper<int>
{
    public int this[int key]
    {
        get
        {
            return aList[key];
        }
        set
        {
            aList[key] = value;
        }
    }
}