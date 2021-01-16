using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterVariablesUI : MonoBehaviour
{
    public TMP_Text healthValue;
    public TMP_Text goldValue;
    public TMP_Text worldState;


    public void UpdateUI()
    {
        int currentHealth = WorldSystem.instance.characterManager.currentHealth;
        int maxHealth = WorldSystem.instance.characterManager.maxHealth;
        int gold = WorldSystem.instance.characterManager.gold;
        healthValue.text = currentHealth.ToString() + "/" + maxHealth.ToString();
        goldValue.text = gold.ToString();

        //DEBUG:
        worldState.text = WorldSystem.instance.worldState.ToString();

    }

}
