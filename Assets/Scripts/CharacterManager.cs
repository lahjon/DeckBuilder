using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    // should match all items from CharacterVariables Enum
    public int gold;
    public int currentHealth;
    public int maxHealth;

    public CharacterVariablesUI characterVariablesUI;

    void Start()
    {
        characterVariablesUI.UpdateUI();
    }

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
            if(currentHealth < 0)
            {
                currentHealth = 0;
                KillCharacter();
            }
            characterVariablesUI.UpdateUI();
        }
    }

    public void KillCharacter()
    {
        Debug.Log("You are dead!");
    }
}

