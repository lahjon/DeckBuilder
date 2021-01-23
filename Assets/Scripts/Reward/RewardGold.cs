using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RewardGold : MonoBehaviour
{
    public TMP_Text text;
    private int goldAmount;

    public float startRange = 5;
    public float endRange = 10;

    public float tierMultiplier = 10;
    public float eliteMultiplier = 1.5f;
    public float bossMultiplier = 2.0f;


    void OnEnable()
    {
        goldAmount = GetGold(WorldSystem.instance.encounterManager.encounterTier, WorldSystem.instance.encounterManager.currentEncounter.encounterType);
        text.text = "Gold: " + goldAmount.ToString();
    }

    private int GetGold(int tier, EncounterType encounterType)
    {
        float multiplier = tierMultiplier;

        switch (encounterType)
        {
            case EncounterType.CombatElite:
                multiplier += eliteMultiplier;
                break;

            case EncounterType.CombatBoss:
                multiplier += bossMultiplier;
                break;
            
            default:
                break;
        }

        float amount = Random.Range(startRange * multiplier,endRange * multiplier);
        return (int)amount;
    }

    public void OnClick()
    {
        WorldSystem.instance.characterManager.gold += goldAmount;
        WorldSystem.instance.characterManager.characterVariablesUI.UpdateUI();
        DestroyImmediate(this.gameObject);
    }
}
