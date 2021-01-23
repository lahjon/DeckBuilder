using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Reward : MonoBehaviour
{
    public float startRange = 5;
    public float endRange = 10;

    public float tierMultiplier = 10;
    public float eliteMultiplier = 1.5f;
    public float bossMultiplier = 2.0f;
    
    public ArtifactData GetArtifact(bool random = true, ArtifactData artifactData = null)
    {
        return null;
    }

    public List<CardData> ChooseCard(CharacterClass characterClass, int amount = 3, bool random = true, CardData cardData = null)
    {
        List<CardData> cardReward = new List<CardData>(); 

        for (int i = 0; i < amount; i++)
        {
            cardReward.Add(DatabaseSystem.instance.GetRandomCard(characterClass));
        }

        return cardReward;
    }

    public int GetGold(int tier, EncounterType encounterType)
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
}
