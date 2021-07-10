using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Event004 : GameEvent
{
    public override void StartGameEvent()
    {
        base.StartGameEvent();
        Debug.Log("Did you just finish you first conversation?!");
        if (WorldSystem.instance.missionManager != null && WorldSystem.instance.missionManager.mission == null)
        {
            WorldSystem.instance.missionManager.NewMission("Mission001", false);
        }
    }  
}