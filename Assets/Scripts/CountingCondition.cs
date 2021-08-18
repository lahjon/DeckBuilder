using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CountingCondition : Condition
{
    int _currentAmount = 0;

    public int requiredAmount => conditionStruct.numValue;

    public virtual int currentAmount
    {
        get => _currentAmount;
        set
        {
            _currentAmount = value;
        }
    }

    public CountingCondition()
    {

    }

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

}
