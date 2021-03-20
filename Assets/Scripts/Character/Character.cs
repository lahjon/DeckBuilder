using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour, ISaveableCharacter
{
    public int                  damage;
    public int                  block;
    public int                  drawCardsAmount;
    public int                  energy;
    public int                  maxHealth;
    public int                  currentHealth;
    public int                  level = 1;
    public int                  experience;
    public CharacterClassType   classType;
    public PlayableCharacterData        characterData;
    public bool                 unlocked;
    public List<string>         selectedTokens = new List<string>();
    public int                  maxCardReward = 3;

    public void SetCharacterData()
    {
        SaveDataManager.LoadJsonData(this.GetComponents<ISaveableCharacter>(), (int)WorldSystem.instance.characterManager.selectedCharacterClassType);
        characterData = WorldSystem.instance.characterManager.allCharacterData[(int)WorldSystem.instance.characterManager.selectedCharacterClassType - 1];
    }

    public void LoadFromSaveDataCharacter(SaveDataCharacter a_SaveData)
    {
        damage = a_SaveData.damageModifier;
        block = a_SaveData.blockModifier;
        drawCardsAmount = a_SaveData.drawCardsAmount;
        energy = a_SaveData.energy;
        maxHealth = a_SaveData.maxHp;
        level = a_SaveData.level;
        classType = a_SaveData.classType;
    }
    public void PopulateSaveDataCharacter(SaveDataCharacter a_SaveData)
    {
        a_SaveData.damageModifier = damage;
        a_SaveData.blockModifier = block;
        a_SaveData.drawCardsAmount = drawCardsAmount;
        a_SaveData.energy = energy;
        a_SaveData.maxHp = maxHealth;
        a_SaveData.level = level;
        a_SaveData.classType = classType;
    }

    public void CreateStartingCharacter(PlayableCharacterData aCharacterData)
    {
        this.maxHealth = aCharacterData.maxHp;
        this.damage = aCharacterData.damageModifier;
        this.block = aCharacterData.blockModifier;
        this.drawCardsAmount = aCharacterData.drawCardsAmount;
        this.energy = aCharacterData.energy;
        this.maxHealth = aCharacterData.maxHp;
        this.level = aCharacterData.level;
        this.classType = aCharacterData.classType;
        this.characterData = aCharacterData;
    }
}