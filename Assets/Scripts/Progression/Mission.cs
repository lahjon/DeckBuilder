using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Mission : Progression
{
    public string description;
    public string startEvent;
    public string nextMission;
    public string overrideMissionGoal = "";

    protected virtual void Start()
    {
        WorldSystem.instance.gameEventManager.StartEvent(startEvent);
        WorldSystem.instance.missionManager.missionUI.UpdateUI(true);
    }
    protected override void Complete()
    {
        Debug.Log("Mission Done");
        Debug.Log(nextMission);
        if (nextMission != null && nextMission != "")
        {
            WorldSystem.instance.missionManager.NewMission(nextMission);
            Debug.Log("Starting new mission");
        }
        else
        {
            WorldSystem.instance.missionManager.ClearMission();
            Debug.Log("No new mission to start in this chain");
        }
    }

}