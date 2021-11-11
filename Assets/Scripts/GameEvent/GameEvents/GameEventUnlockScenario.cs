using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameEventUnlockScenario : GameEvent
{
    public override void TriggerGameEvent()
    { 
        WorldSystem.instance.worldMapManager.UnlockScenario(int.Parse(gameEventStruct.value));
    }
}