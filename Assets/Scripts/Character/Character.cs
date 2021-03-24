using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour, ISaveableCharacter
{
    public int                      damage;
    public int                      block;
    public int                      drawCardsAmount;
    public int                      energy;
    public int                      maxHealth;
    public int                      currentHealth;
    public int                      level = 1;
    public int                      experience;
    public CharacterClassType       classType;
    public PlayableCharacterData    characterData;
    public bool                     unlocked;
    public List<string>             selectedTokens = new List<string>();
    public int                      maxCardReward = 3;
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
        this.maxHealth = aCharacterData.maxHp;
        //this.damage = aCharacterData.damageModifier;
        this.block = aCharacterData.blockModifier;
        this.drawCardsAmount = aCharacterData.drawCardsAmount;
        this.energy = aCharacterData.energy;
        this.maxHealth = aCharacterData.maxHp;
        this.level = aCharacterData.level;
        this.classType = aCharacterData.classType;
        this.characterData = aCharacterData;
    }
}