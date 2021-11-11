using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameEventToggleWorldMap : GameEvent
{
    public override void TriggerGameEvent()
    { 
        world.townManager.worldMapButton.interactable = gameEventStruct.value.ToBool();
    }
}