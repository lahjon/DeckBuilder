using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Mission : Progression
{

    public MissionData data;

    public void StartMission(MissionData aData)
    {
        data = aData;
        aName = data.aName;
        id = data.id;
        description = data.description;

        CreateGoals(data);
        for (int i = 0; i < data.gameEventsOnStart.Count(); i++)
            WorldSystem.instance.gameEventManager.CreateEvent(data.gameEventsOnStart[i]);

        WorldSystem.instance.missionManager.missionUI.UpdateUI(true);
    }
    protected override void Complete()
    {
        base.Complete();
        for (int i = 0; i < data.gameEventsOnEnd.Count(); i++)
            WorldSystem.instance.gameEventManager.CreateEvent(data.gameEventsOnEnd[i]);

        if (data.nextMission != null)
        {
            WorldSystem.instance.missionManager.StartMission(data.nextMission);
            Debug.Log("Starting new mission");
        }

        WorldSystem.instance.missionManager.AddCompleteMission(this);
        Destroy(gameObject);
    }

}