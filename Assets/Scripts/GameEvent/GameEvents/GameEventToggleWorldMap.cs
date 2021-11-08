using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameEventToggleWorldMap : GameEvent
{
    public override void TriggerGameEvent(GameEventStruct gameEventStruct)
    { 
        if (gameEventStruct.value == "true")
            world.townManager.worldMapButton.interactable = true;
        else
            world.townManager.worldMapButton.interactable = false;
    }
}