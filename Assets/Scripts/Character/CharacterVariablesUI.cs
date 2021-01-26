using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterVariablesUI : MonoBehaviour
{
    public TMP_Text healthValue;
    public TMP_Text goldValue;
    public TMP_Text worldState;
    public TMP_Text worldTier;

    public void UpdateUI()
    {
        int currentHealth = WorldSystem.instance.characterManager.currentHealth;
        int maxHealth = WorldSystem.instance.characterManager.maxHealth;
        int gold = WorldSystem.instance.characterManager.gold;
        healthValue.text = currentHealth.ToString() + "/" + maxHealth.ToString();
        goldValue.text = gold.ToString();
        worldTier.text = "Act " + WorldSystem.instance.act.ToString();

        //DEBUG:
        worldState.text = WorldSystem.instance.worldState.ToString();

    }

    public void DisplayDeck()
    {
        WorldSystem.instance.deckDisplayManager.DisplayDeck();
    }

}
