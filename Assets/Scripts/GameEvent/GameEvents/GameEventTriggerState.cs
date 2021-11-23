using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameEventTriggerReward : GameEvent
{
    public override void TriggerGameEvent()
    { 
        WorldStateSystem.SetInState(gameEventStruct.parameter.ToEnum<WorldState>());
    }
}