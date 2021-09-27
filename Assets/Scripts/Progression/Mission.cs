using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Mission : Progression
{
    public string endEvent;
    public string startEvent;
    public MissionData data;

    public void StartMission(MissionData aData)
    {
        data = aData;
        aName = data.aName;
        id = data.id;
        description = data.description;
        endEvent = data.endEvent;
        startEvent = data.startEvent;

        CreateGoals(data);

        WorldSystem.instance.missionManager.missionUI.UpdateUI(true);
        WorldSystem.instance.gameEventManager.StartEvent(startEvent);
    }
    protected override void Complete()
    {
        base.Complete();
        WorldSystem.instance.gameEventManager.StartEvent(endEvent);
        if (data.nextMission != null)
        {
            WorldSystem.instance.missionManager.StartMission(data.nextMission);
            Debug.Log("Starting new mission");
        }

        WorldSystem.instance.missionManager.AddCompleteMission(this);
        Destroy(gameObject);
    }

}