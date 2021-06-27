using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemConditionKillEnemy : ItemCondition, IEvents
{
    public ItemConditionKillEnemy(string aValue, Item anItem) : base(aValue, anItem)
    {
        //requiredAmount = int.Parse(aValue);
    }

    void EnemyKilled(EnemyData enemy)
    {
        currentAmount++;
        CheckCondition();
    }
    public override void Subscribe()
    {
        EventManager.OnEnemyKilledEvent += EnemyKilled;
    }

    public override void Unsubscribe()
    {
        EventManager.OnEnemyKilledEvent -= EnemyKilled;
    }
}
