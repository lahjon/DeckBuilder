using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class SaveDataCharacter
{   
    public int damageModifier;
    public int blockModifier;
    public int drawCardsAmount;
    public int energy;
    public int maxHp;
    public int level;
    public CharacterClassType classType;
    public PlayableCharacterData characterData;
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