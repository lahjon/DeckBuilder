using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
public class GameEventGetReward : GameEvent
{
    public override void TriggerGameEvent()
    { 
        world.rewardManager.CreateReward(gameEventStruct.parameter.ToEnum<RewardType>(), gameEventStruct.value);
    }
}