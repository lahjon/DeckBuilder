using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    // should match all items from CharacterVariables Enum
    public int gold;
    public int currentHealth;
    public int maxHealth;
    
    public List<CardData> playerCardsData;
    public CharacterVariablesUI characterVariablesUI;

    public void AddToCharacter(EncounterOutcomeType type, int value)
    {
        if(type.ToString().ToLower() == "gold")
        {
            gold += value;
            if(gold < 0)
            {
                gold = 0;
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

