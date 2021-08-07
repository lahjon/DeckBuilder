using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BuildingStatsTrackerGoal : ProgressionGoal
{
    public BuildingType buildingGoal;
    public BuildingStatsTrackerGoal(Progression aProgression, BuildingType aBuildingGoal, int aRequiredAmount)
    {
        Subscribe();
        buildingGoal = aBuildingGoal;
        goalName = string.Format("Enter {0}", aBuildingGoal);
        progression = aProgression;
        requiredAmount = aRequiredAmount;
        currentAmount = 0;
    }

    public override void Subscribe()
    {
        base.Subscribe();
        EventManager.OnStatsTrackerUpdatedEvent += StatsTrackedUpdated;
    }
    public override void Unsubscribe()
    {
        base.Unsubscribe();
        EventManager.OnStatsTrackerUpdatedEvent -= StatsTrackedUpdated;
    }
    public override void End()
    {
        Unsubscribe();
        progression.CheckGoals();
        Debug.Log(string.Format("Goal {0} ended.", goalName));
    }

    void StatsTrackedUpdated()
    {
        if (StatsTrackerSystem.buildingTracker.ContainsKey(buildingGoal))
        {
            currentAmount = StatsTrackerSystem.buildingTracker[buildingGoal];
            Evaluate();
        }
        
    }

}