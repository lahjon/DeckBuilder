using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission001 : Mission
{
    protected override void Start()
    {
        name = "Explore the town";
        startEvent = "Event001";
        nextMission = "Mission002";
        completed = true;
        AddGoal(new EnterBuildingGoal(this, BuildingType.TownHall, 1));
        base.Start();
    }
}