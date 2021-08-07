using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Event003 : GameEvent
{
    public override void StartGameEvent()
    {
        base.StartGameEvent();
        world.townManager.worldMapButton.interactable = false;
        if (WorldSystem.instance.missionManager != null && WorldSystem.instance.missionManager.currentMissionId == "Mission001")
        {
            WorldSystem.instance.missionManager.NewMission("Mission001", false);
        }
        Debug.Log("Start event 003");
    }  
}