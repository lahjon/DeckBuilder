using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
public class GameEventGetRewardNormal : GameEvent
{
    public override void TriggerGameEvent()
    { 
        world.rewardManager.CreateRewardNormal(gameEventStruct.parameter.ToEnum<RewardNormalType>(), gameEventStruct.value);
    }
}