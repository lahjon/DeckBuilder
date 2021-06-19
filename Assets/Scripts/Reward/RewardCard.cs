using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardCard : Reward
{
    protected override void CollectCombatReward()
    {
        
        WorldSystem.instance.rewardManager.rewardScreen.currentReward = this.GetComponent<RewardCard>();
        WorldSystem.instance.rewardManager.rewardScreen.rewardScreenCard.GetComponent<RewardScreenCardSelection>().SetupRewards();
    }
    

    public List<CardData> ChooseCard(CharacterClassType characterClass, int amount = 3, bool random = true, CardData cardData = null)
    {
        List<CardData> cardReward = new List<CardData>(); 

        for (int i = 0; i < amount; i++)
        {
            cardReward.Add(DatabaseSystem.instance.GetRandomCard((CardClassType)characterClass));
        }

        return cardReward;
    }
}
