// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using System.Linq;

// public class EnemyStatsTrackerGoal : ProgressionGoal
// {
//     private string enemyId;
//     public EnemyStatsTrackerGoal(Progression aProgression, string anEnemyId, int aRequiredAmount)
//     {
//         Subscribe();
//         enemyId = anEnemyId;
//         requiredAmount = aRequiredAmount;

//         progression = aProgression;
//     }

//     public override void Subscribe()
//     {
//         base.Subscribe();
//         EventManager.OnStatsTrackerUpdatedEvent += StatsTrackedUpdated;
//     }
//     public override void Unsubscribe()
//     {
//         base.Unsubscribe();
//         EventManager.OnStatsTrackerUpdatedEvent -= StatsTrackedUpdated;
//     }
//     public override void End()
//     {
//         Unsubscribe();
//         progression.CheckGoals();
//         Debug.Log(string.Format("Goal {0} ended.", goalName));
//     }

//     void StatsTrackedUpdated()
//     {
//         if (StatsTrackerSystem.enemyTracker.ContainsKey(enemyId))
//         {
//             currentAmount = StatsTrackerSystem.enemyTracker[enemyId];
//             Evaluate();
//         }
        
//     }

// }