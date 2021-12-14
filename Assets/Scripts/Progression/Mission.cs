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
            GameEventManager.CreateEvent(data.gameEventsOnStart[i]);

        WorldSystem.instance.missionManager.missionUI.UpdateUI(true);
    }
    protected override void Complete()
    {
        base.Complete();
        WorldSystem.instance.uiManager.UIWarningController.CreateWarning(string.Format("Mission {0} completed!", data.aName));
        for (int i = 0; i < data.gameEventsOnEnd.Count(); i++)
            GameEventManager.CreateEvent(data.gameEventsOnEnd[i]);

        if (DatabaseSystem.instance.missions.Where(x => x.id == data.nextMissionId).FirstOrDefault() is MissionData md)
        {
            WorldSystem.instance.missionManager.StartMission(md);
            Debug.Log("Starting new mission");
        }

        WorldSystem.instance.missionManager.AddCompleteMission(this);
        Destroy(gameObject);
    }

}