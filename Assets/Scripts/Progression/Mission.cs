using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Mission : Progression
{
    public string missionId;
    public string description;
    public string endEvent;
    public string nextMission;
    public string overrideMissionGoal = "";

    protected virtual void Start()
    {
        WorldSystem.instance.missionManager.missionUI.UpdateUI(true);
    }
    protected override void Complete()
    {
        Debug.Log("Mission Done");
        WorldSystem.instance.gameEventManager.StartEvent(endEvent);
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