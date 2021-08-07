using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Event001 : GameEvent
{
    public override void StartGameEvent()
    {
        WorldSystem.instance.townManager.GetTownInteractableByType(BuildingType.TownHall)?.HighlightBuilding();
        Debug.Log("Start Event001!");
    }
}