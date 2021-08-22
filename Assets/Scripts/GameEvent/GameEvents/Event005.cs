using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Event005 : GameEvent
{
    public override void StartGameEvent()
    {
        base.StartGameEvent();
        WorldStateSystem.SetInTownReward(true);
        world.rewardManager.CreateReward(RewardType.Item);
    }  
}