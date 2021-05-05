using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class KillEnemyGoal : ProgressionGoal, IEvents
{
    private string enemyId;
    private string enemyName;

    public KillEnemyGoal(Progression aProgression, int aRequiredAmount, string aName, string anId)
    {
        Subscribe();
        currentAmount = 0;
        enemyId = anId;
        enemyName = aName;
        requiredAmount = aRequiredAmount;
        goalName = string.Format("Kill {0} {1}/{2}", enemyName, currentAmount, requiredAmount);
        progression = aProgression;
    }
    public void Subscribe()
    {
        EventManager.OnEnemyKilledEvent += EnemyKilled;
    }
    public void Unsubscribe()
    {
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
            goalName = string.Format("Kill {0} {1}/{2}", enemyName, currentAmount, requiredAmount);
        }
    }

}