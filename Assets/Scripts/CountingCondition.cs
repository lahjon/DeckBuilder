using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CountingCondition : Condition
{
    public int currentAmount = 0;

    public int requiredAmount => conditionStruct.numValue;

    public CountingCondition(ConditionStruct conditionStruct, Action OnPreConditionUpdate, Action OnConditionFlipTrue) 
        : base(conditionStruct,OnPreConditionUpdate, null, OnConditionFlipTrue)
    {
        ConditionEvaluator = GreaterThanComparer;
    }

    public bool GreaterThanComparer(ConditionStruct conditionStruct)
    {
        return currentAmount >= conditionStruct.numValue;
    }

    public override void OnEventNotification()
    {
        Debug.Log("In condition: " + ConditionEvaluator);
        if (ConditionEvaluator == null) return; //dodges initial notification OnCreation
        currentAmount++;
        base.OnEventNotification();
    }

    public override void OnEventNotification(EnemyData enemy)
    {
        Debug.Log("Enemy Killed " + this);
        if (enemy.enemyId == conditionStruct.strParameter || string.IsNullOrEmpty(conditionStruct.strParameter))
        {
            Debug.Log("adding");
            OnEventNotification();
        }
    }

    public string GetDescription(bool getCurrentAmount) => conditionStruct.type switch
    {
        ConditionType.None => "No Condition",
        ConditionType.KillEnemy => string.Format("<b>Kill Enemies (" + (getCurrentAmount ? Mathf.Abs(currentAmount - requiredAmount) : requiredAmount) + ")</b>"),
        ConditionType.ClearTile => string.Format("<b>Clear Tiles (" + (getCurrentAmount ? Mathf.Abs(currentAmount - requiredAmount) : requiredAmount) + ")</b>"),
        ConditionType.WinCombat => string.Format("<b>Win Combats (" + (getCurrentAmount ? Mathf.Abs(currentAmount - requiredAmount) : requiredAmount) + ")</b>"),
        _ => null
    };

}
