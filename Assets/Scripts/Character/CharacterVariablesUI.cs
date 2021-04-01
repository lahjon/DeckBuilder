using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterVariablesUI : MonoBehaviour
{
    public TMP_Text healthValue;
    public TMP_Text goldValue;
    public TMP_Text shardValue;
    public TMP_Text worldTier;
    public Image levelUpImage;
    public GameObject leftBar;

    public void UpdateCharacterHUD()
    {
        if (WorldSystem.instance.characterManager != null && WorldSystem.instance.characterManager.character != null)
        {
            int currentHealth = WorldSystem.instance.characterManager.currentHealth;
            int maxHealth = WorldSystem.instance.characterManager.characterStats.GetStat(StatType.Health);
            int gold = WorldSystem.instance.characterManager.gold;
            int shards = WorldSystem.instance.characterManager.shard;
            healthValue.text = currentHealth.ToString() + "/" + maxHealth.ToString();
            goldValue.text = gold.ToString();
            shardValue.text = shards.ToString();
        }
    }

    public void ShowLeftBar()
    {
        Debug.Log("Show");
    }

    public void HideLeftBar()
    {
        Debug.Log("Hide");
    }

    public void ActivateLevelUp()
    {
        levelUpImage.gameObject.SetActive(true);
        LeanTween.scale(levelUpImage.gameObject, new Vector3(0.8f, 0.8f, 0.8f), 0.5f).setEaseInBounce().setLoopPingPong();
    }

    public void DisableLevelUp()
    {
        levelUpImage.gameObject.SetActive(false);
    }

    public void ButtonDisplayDeck()
    {
        WorldStateSystem.SetInDisplay();
    }

    public void ButtonOpenCharacterSheet()
    {
        WorldStateSystem.SetInCharacterSheet();
    }
}
