using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission02 : Mission
{
    protected override void Start()
    {
        startEvent = "Event02";
        nextMission = "Mission02";
        completed = true;
        goals.Add(new EnterBuildingGoal(BuildingType.TownHall));
        base.Start();
    }
}