using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Mission : Progression
{
    public string description;
    public string endEvent;
    public string startEvent;
    public string nextMission;

    public void StartMission(MissionData data)
    {
        goals.Clear();
        goalsTrackAmount.Clear();

        aName = data.aName;
        id = data.id;
        description = data.description;
        endEvent = data.endEvent;
        startEvent = data.startEvent;
        nextMission = data.nextMission;

        #region goals

        // enter building
        data?.goalEnterBuilding.ForEach(x => AddGoal(new EnterBuildingGoal(this, x.buildingType, x.requiredAmount)));

        // kill enemy
        data?.goalKillEnemy.ForEach(x => AddGoal(new KillEnemyGoal(this, x.enemyId, x.requiredAmount)));

        #endregion

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