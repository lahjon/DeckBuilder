using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterVariablesUI : MonoBehaviour
{
    public TMP_Text healthValue;
    public TMP_Text goldValue;
    public TMP_Text shardValue;
    public TMP_Text worldTier;

    public void UpdateCharacterHUD()
    {
        if (WorldSystem.instance.characterManager != null && WorldSystem.instance.characterManager.character != null)
        {
            int currentHealth = WorldSystem.instance.characterManager.character.currentHealth;
            int maxHealth = WorldSystem.instance.characterManager.character.maxHealth;
            int gold = WorldSystem.instance.characterManager.gold;
            int shards = WorldSystem.instance.characterManager.shard;
            healthValue.text = currentHealth.ToString() + "/" + maxHealth.ToString();
            goldValue.text = gold.ToString();
            shardValue.text = shards.ToString();
        }
    }

    public void DisplayDeck()
    {
        WorldStateSystem.SetInDisplay();
    }

}
