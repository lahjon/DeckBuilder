using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective002 : Objective
{
    public override void Init()
    {
        name = "Complete your second mission";
        description = "You have completed your second mission! Congratulations!";
        //completeEvent = "Event003";
        AddGoal(new BuildingStatsTrackerGoal(this, BuildingType.Barracks, 5));
    }

    protected override void TriggerEvent()
    {
    }
}