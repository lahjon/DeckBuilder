using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission01 : Mission
{
    protected override void Start()
    {
        missionName = "Explore the town";
        startEvent = "Event01";
        nextMission = "Mission02";
        completed = true;
        goals.Add(new EnterBuildingGoal(BuildingType.TownHall));
        base.Start();
    }
}