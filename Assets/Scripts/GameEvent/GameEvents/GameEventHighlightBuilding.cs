using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameEventHighlightBuilding : GameEvent
{
    public override void TriggerGameEvent()
    { 
        WorldSystem.instance.townManager.GetTownInteractableByType(gameEventStruct.parameter.ToEnum<BuildingType>()).Highlighted = gameEventStruct.value.ToBool();
    }
}