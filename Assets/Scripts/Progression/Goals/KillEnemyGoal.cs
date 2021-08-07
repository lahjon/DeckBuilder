using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class KillEnemyGoal : ProgressionGoal
{
    private string enemyId;

    public KillEnemyGoal(Progression aProgression, string anId, int aRequiredAmount)
    {
        Subscribe();
        currentAmount = 0;
        enemyId = anId;
        requiredAmount = aRequiredAmount;
        goalName = string.Format("Kill {0} {1}/{2}", enemyId, currentAmount, requiredAmount);
        progression = aProgression;
    }
    public override void Subscribe()
    {
        base.Subscribe();
        EventManager.OnEnemyKilledEvent += EnemyKilled;
    }
    public override void Unsubscribe()
    {
        base.Unsubscribe();
        EventManager.OnEnemyKilledEvent -= EnemyKilled;
    }
    public override void End()
    {
        Unsubscribe();
        progression.CheckGoals();
        Debug.Log(string.Format("Goal {0} ended.", goalName));
    }

    void EnemyKilled(EnemyData enemyData)
    {
        //Debug.Log("Enemy died with ID: " + enemyData.enemyName);
        if (enemyData.enemyId == enemyId)
        {
            currentAmount++;
            Evaluate();
            goalName = string.Format("Kill {0} {1}/{2}", enemyId, currentAmount, requiredAmount);
        }
    }

}