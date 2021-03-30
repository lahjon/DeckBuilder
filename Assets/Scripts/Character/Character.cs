using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Character : MonoBehaviour, ISaveableCharacter
{
    public int                      level;
    public int                      experience;
    public CharacterClassType       classType;
    public PlayableCharacterData    characterData;
    public bool                     unlocked;
    public List<string>             selectedTokens = new List<string>();
    CharacterStats characterStats;
    bool initialized;

    public void SetCharacterData(int index, CharacterData characterData = null)
    {
        if (!initialized)
        {

            if (characterData == null)
            {
                SaveDataManager.LoadJsonData(GetComponents<ISaveableCharacter>(), index);
                characterData = WorldSystem.instance.characterManager.allCharacterData[(int)WorldSystem.instance.characterManager.selectedCharacterClassType - 1];
            }

            if (level == 0)
            {
                level = 1;
            }

            characterStats = gameObject.GetComponent<CharacterStats>();
            initialized = true;
            characterStats.Init();
        }
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