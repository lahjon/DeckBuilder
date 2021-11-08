using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameEventGetRewardAndTrigger : GameEvent
{
    public override void TriggerGameEvent(GameEventStruct gameEventStruct)
    { 
        if (int.Parse(gameEventStruct.parameter) is int anInt && (RewardType)anInt is RewardType rewardType)
        {
            world.rewardManager.CreateReward(rewardType, gameEventStruct.value);
            world.rewardManager.TriggerReward();
        }  
    }
}