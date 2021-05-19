using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RewardGold : Reward
{
    public TMP_Text text;
    private int goldAmount;

    public float startRange = 5;
    public float endRange = 10;

    public float tierMultiplier = 1;
    public float eliteMultiplier = 1.5f;
    public float bossMultiplier = 2.0f;


    void OnEnable()
    {
        goldAmount = GetGold(WorldSystem.instance.encounterManager.encounterTier, WorldSystem.instance.encounterManager.currentEncounter.encounterType);
        text.text = "Gold: " + goldAmount.ToString();
    }

    private int GetGold(int tier, OverworldEncounterType encounterType)
    {
        float multiplier = tier * tierMultiplier;

        switch (encounterType)
        {
            case OverworldEncounterType.CombatElite:
                multiplier += eliteMultiplier;
                break;

            case OverworldEncounterType.CombatBoss:
                multiplier += bossMultiplier;
                break;
            
            default:
                break;
        }

        float amount = Random.Range(startRange * multiplier,endRange * multiplier);
        return (int)amount;
    }

    protected override void CollectCombatReward()
    {
        Debug.Log(goldAmount);
        WorldSystem.instance.characterManager.gold += goldAmount;
    }
}

