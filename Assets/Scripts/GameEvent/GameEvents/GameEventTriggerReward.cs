using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameEventTriggerReward : GameEvent
{
    public override void TriggerGameEvent()
    { 
        world.rewardManager.TriggerReward();
    }
}