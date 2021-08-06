using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Character : MonoBehaviour, ISaveableCharacter
{
    public int level;
    public int experience;
    public CharacterClassType classType;
    public PlayableCharacterData characterData;
    public bool unlocked;
    public List<string> selectedTokens = new List<string>();
    public Profession profession;
    CharacterStats characterStats;

    public void SetCharacterData(int index)
    {
        SaveDataManager.LoadJsonData(GetComponents<ISaveableCharacter>(), index);
        characterData = WorldSystem.instance.characterManager.allCharacterData[index];

        if (level == 0)
        {
            level = 1;
        }

        characterStats = gameObject.GetComponent<CharacterStats>();
        characterStats.Init();
    }

    public void LoadFromSaveDataCharacter(SaveDataCharacter a_SaveData)
    {
        level = a_SaveData.level;
        profession = a_SaveData.profession;
    }
    public void PopulateSaveDataCharacter(SaveDataCharacter a_SaveData)
    {
        a_SaveData.level = level;
        a_SaveData.profession = profession;
    }
}