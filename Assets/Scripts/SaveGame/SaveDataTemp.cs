using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class SaveDataTemp
{
    public List<CardWrapper> playerCards = new List<CardWrapper>();
    public CharacterClassType selectedCharacterClassType;
    public List<string> selectedTokens = new List<string>(); 
    public List<string> allActiveArtifactsNames = new List<string>();
    public List<string> selectedUseItems = new List<string>(); 
    public int gold;
    public int currentHealth;
    public int addedHealth;
    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public void LoadFromJson(string a_Json)
    {
        JsonUtility.FromJsonOverwrite(a_Json, this);
    }
}

public interface ISaveableTemp
{
    void PopulateSaveDataTemp(SaveDataTemp a_SaveData);
    void LoadFromSaveDataTemp(SaveDataTemp a_SaveData);
}