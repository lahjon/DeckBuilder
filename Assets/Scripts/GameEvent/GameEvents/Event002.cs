using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Event002 : GameEvent
{
    public override void StartGameEvent()
    {
        base.StartGameEvent();
        world.dialogueManager.SetDataFromString("DemoStart02");
        WorldSystem.instance.townManager.GetTownInteractableByType(BuildingType.TownHall)?.UnhighlightBuilding();
        world.townManager.worldMapButton.interactable = true;

        Debug.Log("Start Event002!");
    }  
}