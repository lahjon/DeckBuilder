using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ConditionCounting : Condition
{
    public int currentAmount = 0;
    public ConditionCountingOnTrueType onTrueType;
    public ConditionType resetConditionType;
 
    public int requiredAmount => conditionData.numValue;

    public ConditionCounting(ConditionData conditionData, Action OnPreConditionUpdate, Action OnConditionFlipTrue, ConditionCountingOnTrueType onTrueType = ConditionCountingOnTrueType.Nothing, ConditionType resetCondition = ConditionType.None) 
        : base(conditionData,OnPreConditionUpdate, null, OnConditionFlipTrue)
    {
        ConditionEvaluator = GreaterThanComparer;
        this.resetConditionType = resetCondition;
        this.onTrueType = onTrueType;
    }

    public bool GreaterThanComparer(ConditionData conditionData)
    {
        return currentAmount >= conditionData.numValue;
    }

    private void Reset() => currentAmount = 0;

    public override void OnEventNotification()
    {
        Debug.Log("notified");
        if (ConditionEvaluator == null) return; //dodges initial notification OnCreation
        currentAmount++;
        bool preVal = value;
        base.OnEventNotification();
        if(!preVal && value)
        {
            if (onTrueType == ConditionCountingOnTrueType.Reset)
            {
                currentAmount = 0;
                value = false;
            }
            else if (onTrueType == ConditionCountingOnTrueType.Unsubscribe)
                Unsubscribe();
        }
    }

    public string GetDescription(bool getCurrentAmount) => conditionData.type switch
    {
        ConditionType.None => "No Condition",
        ConditionType.KillEnemy => string.Format("<b>Kill Enemies (" + (getCurrentAmount ? Mathf.Abs(currentAmount - requiredAmount) : requiredAmount) + ")</b>"),
        ConditionType.ClearTile => string.Format("<b>Clear Tiles (" + (getCurrentAmount ? Mathf.Abs(currentAmount - requiredAmount) : requiredAmount) + ")</b>"),
        ConditionType.WinCombat => string.Format("<b>Win Combats (" + (getCurrentAmount ? Mathf.Abs(currentAmount - requiredAmount) : requiredAmount) + ")</b>"),
        _ => null
    };

    public override void Subscribe()
    {
        base.Subscribe();
        SubscribeResetEvent();
    }

    private void SubscribeResetEvent()
    {
        switch (resetConditionType)
        {
            case ConditionType.None:
                return;
            case ConditionType.EndTurn:
                EventManager.OnTurnEndEvent += Reset;
                break;
            default:
                break;
        }
    }

    private void UnsubscribeResetEvent()
    {
        switch (resetConditionType)
        {
            case ConditionType.None:
                return;
            case ConditionType.EndTurn:
                EventManager.OnTurnEndEvent -= Reset;
                break;
            default:
                break;
        }
    }

}

public enum ConditionCountingOnTrueType
{
    Nothing,
    Reset,
    Unsubscribe
}