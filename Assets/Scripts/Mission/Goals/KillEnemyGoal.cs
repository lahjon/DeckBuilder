using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class KillEnemyGoal : MissionGoal
{
    private string enemyId;
    private string enemyName;

    public KillEnemyGoal(int aRequiredAmount, string aName, string anId)
    {
        Init();
        currentAmount = 0;
        enemyId = anId;
        enemyName = aName;
        requiredAmount = aRequiredAmount;
        goalName = string.Format("Kill {0} {1}/{2}", enemyName, currentAmount, requiredAmount);
    }
    public override void Init()
    {
        EventManager.OnEnemyKilledEvent += EnemyKilled;
    }
    public override void End()
    {
        EventManager.OnEnemyKilledEvent -= EnemyKilled;
        base.End();
    }

    void EnemyKilled(EnemyData enemyData)
    {
        Debug.Log("Enemy died with ID: " + enemyData.enemyName);
        if (enemyData.enemyId == enemyId)
        {
            currentAmount++;
            Evaluate();
            goalName = string.Format("Kill {0} {1}/{2}", enemyName, currentAmount, requiredAmount);
        }
    }
}