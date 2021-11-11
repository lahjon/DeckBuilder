using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameEventTriggerState : GameEvent
{
    public override void TriggerGameEvent()
    { 
        world.rewardManager.TriggerReward();
    }
}