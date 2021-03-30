using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class SaveDataStart
{
    public CharacterClassType characterClassType;
    public List<string> selectedTokens = new List<string>(); 
    public List<CharacterClassType> unlockedCharacters = new List<CharacterClassType>(); 
    
    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public void LoadFromJson(string a_Json)
    {
        JsonUtility.FromJsonOverwrite(a_Json, this);
    }
}

public interface ISaveableStart
{
    void PopulateSaveDataStart(SaveDataStart a_SaveData);
    void LoadFromSaveDataStart(SaveDataStart a_SaveData);
}