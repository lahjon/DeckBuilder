using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Event003 : GameEvent
{
    public override void StartGameEvent()
    {
        base.StartGameEvent();
        world.townManager.worldMapButton.interactable = false;
        WorldSystem.instance.missionManager.StartMission("Mission001");
        Debug.Log("Start event 003");
    }  
}