using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour, ISaveableCharacter
{
    public int                      level;
    public int                      experience;
    public CharacterClassType       classType;
    public PlayableCharacterData    characterData;
    public bool                     unlocked;
    public List<string>             selectedTokens = new List<string>();
    CharacterStats characterStats;

    public void SetCharacterData()
    {
        SaveDataManager.LoadJsonData(GetComponents<ISaveableCharacter>(), (int)WorldSystem.instance.characterManager.selectedCharacterClassType);
        characterData = WorldSystem.instance.characterManager.allCharacterData[(int)WorldSystem.instance.characterManager.selectedCharacterClassType - 1];
        characterStats = gameObject.GetComponent<CharacterStats>();
        characterStats.Init();
    }

    public void LoadFromSaveDataCharacter(SaveDataCharacter a_SaveData)
    {
        level = a_SaveData.level;
    }
    public void PopulateSaveDataCharacter(SaveDataCharacter a_SaveData)
    {
        a_SaveData.level = level;
    }

    public void CreateStartingCharacter(PlayableCharacterData aCharacterData)
    {
        this.classType = aCharacterData.classType;
        this.characterData = aCharacterData;
    }
}