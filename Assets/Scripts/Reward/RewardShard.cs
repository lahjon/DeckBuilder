using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RewardShard : Reward
{
    public TMP_Text text;
    private int shardAmount;

    public float startRange = 1;
    public float endRange = 2;

    public float tierMultiplier = 1;
    public float eliteMultiplier = 1.5f;
    public float bossMultiplier = 2.0f;


    void OnEnable()
    {
        shardAmount = GetShards(WorldSystem.instance.encounterManager.encounterTier, WorldSystem.instance.encounterManager.currentEncounter.encounterType);
        text.text = "Shards: " + shardAmount.ToString();
    }

    private int GetShards(int tier, OverworldEncounterType encounterType)
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
        Debug.Log(shardAmount);
        WorldSystem.instance.characterManager.shard += shardAmount;
    }
}

