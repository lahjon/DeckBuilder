using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameEventHighlightBuilding : GameEvent
{
    public override void TriggerGameEvent()
    { 
        
        if (int.Parse(gameEventStruct.parameter) is int anInt && (BuildingType)anInt is BuildingType buildingType)
            if (gameEventStruct.value == "true")
                WorldSystem.instance.townManager.GetTownInteractableByType(buildingType).Highlighted = true;
            else
                WorldSystem.instance.townManager.GetTownInteractableByType(buildingType).Highlighted = false;
    }
}