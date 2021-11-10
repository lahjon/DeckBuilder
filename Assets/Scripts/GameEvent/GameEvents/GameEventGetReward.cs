using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameEventGetReward : GameEvent
{
    public override void TriggerGameEvent()
    { 
        if (int.Parse(gameEventStruct.parameter) is int anInt && (RewardType)anInt is RewardType rewardType)
            world.rewardManager.CreateReward(rewardType, gameEventStruct.value);
    }
}