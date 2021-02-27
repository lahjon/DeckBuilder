using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyStatsTrackerGoal : ProgressionGoal, IEvents
{
    private string enemyId;
    public EnemyStatsTrackerGoal(Progression aProgression, string anEnemyId, int aRequiredAmount)
    {
        Subscribe();
        enemyId = anEnemyId;
        requiredAmount = aRequiredAmount;

        progression = aProgression;
    }

    public void Subscribe()
    {
        EventManager.OnStatsTrackerUpdatedEvent += StatsTrackedUpdated;
    }
    public void Unsubscribe()
    {
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
        if (StatsTracker.enemyTracker.ContainsKey(enemyId))
        {
            currentAmount = StatsTracker.enemyTracker[enemyId];
            Evaluate();
        }
        
    }

}