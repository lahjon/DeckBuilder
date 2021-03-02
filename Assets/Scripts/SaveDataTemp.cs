using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class SaveDataTemp
{
    public CharacterClassType characterClassType;
    public List<string> playerCardsDataNames = new List<string>();
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