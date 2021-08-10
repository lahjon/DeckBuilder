using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class SaveDataCharacter
{   
    public int level;
    public Profession profession;
    public List<string> currentCards;
    public List<string> sideCards;
    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public void LoadFromJson(string a_Json)
    {
        JsonUtility.FromJsonOverwrite(a_Json, this);
    }
}
public interface ISaveableCharacter
{
    void PopulateSaveDataCharacter(SaveDataCharacter a_SaveData);
    void LoadFromSaveDataCharacter(SaveDataCharacter a_SaveData);
}