using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class SaveDataTemp
{
    public List<CardWrapper> playerCards = new List<CardWrapper>();
    public CharacterClassType selectedCharacterClassType;
    public ProfessionType currentProfessionType;
    public EquipmentStruct equipmentStruct;
    public List<int> selectedTokens = new List<int>(); 
    public List<int> allActiveArtifactsNames = new List<int>();
    public List<int> selectedUseItems = new List<int>(); 
    public int gold;
    public int embers;
    //public int currentHealth;
    //public int addedHealth;
    
    public int damageTaken;
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