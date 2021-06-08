using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class Mission : Progression
{
    public string missionId;
    public string description;
    public string endEvent;
    public string startEvent;
    public string nextMission;
    public string overrideMissionGoal;

    // public abstract string missionId
    // {
    //     get; set;
    // }
    // public string description
    // {
    //     get => _description;
    //     set => _description = value;
    // }
    // public string endEvent
    // {
    //     get => _endEvent;
    //     set => _endEvent = value;
    // }
    // public string startEvent
    // {
    //     get => _startEvent;
    //     set => _startEvent = value;
    // }
    // public string nextMission
    // {
    //     get => _nextMission;
    //     set => _nextMission = value;
    // }
    // public string overrideMissionGoal
    // {
    //     get => _overrideMissionGoal;
    //     set => _overrideMissionGoal = value;
    // }

    protected virtual void Start()
    {
        WorldSystem.instance.missionManager.missionUI.UpdateUI(true);
        WorldSystem.instance.gameEventManager.StartEvent(startEvent);
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