using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardCard : Reward
{
    protected override void CollectCombatReward()
    {
        
        WorldSystem.instance.uiManager.rewardScreen.currentReward = this.GetComponent<RewardCard>();
        WorldSystem.instance.uiManager.rewardScreen.rewardScreenCard.GetComponent<RewardScreenCard>().SetupRewards();
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
}
