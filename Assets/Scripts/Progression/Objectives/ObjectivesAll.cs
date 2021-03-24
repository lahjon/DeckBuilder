using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectivesAll
{
    public HashSet<Objective> objectives = new HashSet<Objective>();
    public void AddObjectives()
    {
        Objective obj = new Objective();

        obj.progressName = "Obj0001";
        obj.description = "This is your first objective!";
        obj.completeEvent = "";
        obj.goals.Add(new BuildingStatsTrackerGoal(obj, BuildingType.Barracks, 5));
        objectives.Add(obj);

        obj = new Objective();
        obj.progressName = "Obj0002";
        obj.description = "This is your second objective!";
        obj.completeEvent = "";
        obj.goals.Add(new BuildingStatsTrackerGoal(obj, BuildingType.Barracks, 5));
        objectives.Add(obj);

        obj = new Objective();
        obj.progressName = "Obj0003";
        obj.description = "This is your third objective!";
        obj.completeEvent = "";
        obj.goals.Add(new BuildingStatsTrackerGoal(obj, BuildingType.Barracks, 5));
        objectives.Add(obj);
    }
}