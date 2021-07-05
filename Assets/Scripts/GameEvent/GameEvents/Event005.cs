using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Event005 : GameEvent
{
    public override void StartGameEvent()
    {
        base.StartGameEvent();
        world.rewardManager.rewardScreen.GetReward(RewardType.Artifact);
    }  
}