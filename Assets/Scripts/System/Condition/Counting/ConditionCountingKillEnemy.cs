using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ConditionCountingKillEnemy : ConditionCounting
{

    public override void Subscribe() {
        base.Subscribe();
        EventManager.OnEnemyKilledEvent += CheckValid; 
    }

    public override void Unsubscribe()
    {
        base.Unsubscribe();
        EventManager.OnEnemyKilledEvent -= CheckValid;
    }

    public void CheckValid(CombatActorEnemy enemy)
    {
        if (string.IsNullOrEmpty(conditionData.strParameter) || enemy.enemyData.enemyId == conditionData.strParameter.ToInt())
            OnEventNotification();
    }

}