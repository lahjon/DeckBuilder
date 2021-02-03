using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharacterManager : MonoBehaviour
{
    // should match all items from CharacterVariables Enum
    private int _gold;
    public int startingGold = 99;
    public int currentHealth;
    public int maxHealth;
    public int maxCardReward = 3;
    public CharacterClass characterClass;
    public int startEnergy;
    public int startDrawAmount;
    public CharacterData characterData;

    public Dictionary<CharacterStat, int> stats = new Dictionary<CharacterStat, int>();
    public int damageModifier, blockModifier, handSize, cardDrawAmount, energy, magicFind;
    
    public List<CardData> playerCardsData;
    public CharacterVariablesUI characterVariablesUI;

    void Start()
    {
        BindCharacterData();
        characterVariablesUI.UpdateUI();
    }

    public int gold 
    {
        get
        {
            return _gold;
        }
        set
        {
            _gold += value;
            characterVariablesUI.UpdateUI();
        }
    }

    public void Reset()
    {
        BindCharacterData();
    }

    private void BindCharacterData()
    {
        playerCardsData.Clear();
        stats.Clear();

        _gold = characterData.gold;
        maxHealth = characterData.maxHealth;
        startDrawAmount = characterData.drawAmount;
        startEnergy = characterData.energy;
        characterClass = characterData.characterClass;
        characterData.startingDeck.allCards.ForEach(x => playerCardsData.Add(x));

        foreach(CharacterStat stat in System.Enum.GetValues(typeof(CharacterStat)) )
        {
            stats.Add(stat, 0);
        }
        currentHealth = maxHealth;
    }


    public void AddToCharacter(EncounterOutcomeType type, int value)
    {
        if(type.ToString().ToLower() == "gold")
        {
            _gold += value;
            if(_gold < 0)
            {
                _gold = 0;
            }
            characterVariablesUI.UpdateUI();
        }
        else if(type.ToString().ToLower() == "health")
        {
            currentHealth += value;
            if(currentHealth < 1)
            {
                currentHealth = 0;
                KillCharacter();
            }
            characterVariablesUI.UpdateUI();
        }
    }

    public void AddStat(CharacterStat stat, int value)
    {
        stats[stat] += value; 
        UpdateModifiers();
    }


    public void UpdateModifiers()
    {
        damageModifier = (int)stats[CharacterStat.Strength] / 3;
        blockModifier = (int)stats[CharacterStat.Endurance] / 3;
        energy = startEnergy + (int)stats[CharacterStat.Wisdom] / 5;
        cardDrawAmount = startDrawAmount + (int)stats[CharacterStat.Speed] / 5;
        magicFind = (int)stats[CharacterStat.Cunning] * 2;
    } 

    public void AddCardDataToDeck(CardData newCardData)
    {
        playerCardsData.Add(newCardData); 
        WorldSystem.instance.deckDisplayManager.UpdateAllCards();
    }
    public void RemoveCardDataFromDeck(int index)
    {
        if(playerCardsData.Count > 1)
        {
            playerCardsData.RemoveAt(index);
            WorldSystem.instance.deckDisplayManager.RemoveCardAtIndex(index);
        }
        else
        {
            Debug.Log("No more cards to remove!");
        }
    }

    public void KillCharacter()
    {
        Debug.Log("You are dead!");
    }
}

