using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective001 : Objective
{
    public override void Init()
    {
        name = "Complete your first mission";
        description = "You have completed your first mission! Congratulations!";
        //completeEvent = "Event003";
        AddGoal(new EnterBuildingGoal(this, BuildingType.TownHall, 4));
    }

    protected override void TriggerEvent()
    {
    }
}