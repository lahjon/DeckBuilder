using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour, ISaveableCharacter
{
    public int                  damageModifier;
    public int                  blockModifier;
    public int                  drawCardsAmount;
    public int                  energy;
    public int                  maxHp;
    public int                  level;
    public CharacterClassType   classType;
    public CharacterData        characterData;
    public bool                 unlocked;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void LoadFromSaveDataCharacter(SaveDataCharacter a_SaveData)
    {
        damageModifier = a_SaveData.damageModifier;
        blockModifier = a_SaveData.blockModifier;
        drawCardsAmount = a_SaveData.drawCardsAmount;
        energy = a_SaveData.energy;
        maxHp = a_SaveData.maxHp;
        level = a_SaveData.level;
        classType = a_SaveData.classType;
        characterData = a_SaveData.characterData;
    }
    public void PopulateSaveDataCharacter(SaveDataCharacter a_SaveData)
    {
        a_SaveData.damageModifier = damageModifier;
        a_SaveData.blockModifier = blockModifier;
        a_SaveData.drawCardsAmount = drawCardsAmount;
        a_SaveData.energy = energy;
        a_SaveData.maxHp = maxHp;
        a_SaveData.level = level;
        a_SaveData.classType = classType;
        a_SaveData.characterData = characterData;
    }

    public void CreateStartingCharacter(CharacterData aCharacterData)
    {
        this.maxHp = aCharacterData.maxHp;
        this.damageModifier = aCharacterData.damageModifier;
        this.blockModifier = aCharacterData.blockModifier;
        this.drawCardsAmount = aCharacterData.drawCardsAmount;
        this.energy = aCharacterData.energy;
        this.maxHp = aCharacterData.maxHp;
        this.level = aCharacterData.level;
        this.classType = aCharacterData.classType;
        this.characterData = aCharacterData;
    }
}