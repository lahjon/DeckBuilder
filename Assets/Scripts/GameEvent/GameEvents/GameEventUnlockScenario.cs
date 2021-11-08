using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameEventUnlockScenario : GameEvent
{
    public override void TriggerGameEvent(GameEventStruct gameEventStruct)
    { 
        if (int.Parse(gameEventStruct.value) is int idx)
            WorldSystem.instance.worldMapManager.UnlockScenario(idx);
    }
}