using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ConditionCounting : Condition
{
    private int _currentAmount = 0;

    public int currentAmount
    {
        get => _currentAmount;
        set
        {
            _currentAmount = value;
            onCurrentAmountChanged?.Invoke();
        }
    }

    public ConditionCountingOnTrueType onTrueType;
    public ConditionType resetConditionType;

    public Action onCurrentAmountChanged;
 
    public int requiredAmount => conditionData.numValue;

    public static ConditionCounting Factory(ConditionData conditionData, IConditionOwner owner, Action onCurrentAmountChanged, Action OnConditionFlipTrue, ConditionCountingOnTrueType onTrueType = ConditionCountingOnTrueType.Nothing, ConditionType resetCondition = ConditionType.None)
    {
        if (conditionData == null) return new ConditionCountingNotConfigured();
        ConditionCounting cond = Helpers.InstanceObject<ConditionCounting>(string.Format("ConditionCounting{0}", conditionData.type));
        if (cond is null) return new ConditionCountingNotConfigured();

        cond.conditionData = conditionData;
        cond.owner = owner;
        cond.OnConditionFlipTrue = OnConditionFlipTrue;
        cond.onCurrentAmountChanged = onCurrentAmountChanged;
        cond.resetConditionType = resetCondition;
        cond.onTrueType = onTrueType;
        return cond;
    }

    public override bool ConditionEvaluator()
    {
        return currentAmount >= conditionData.numValue;
    }

    private void Reset() => currentAmount = 0;

    public override void OnEventNotification()
    {
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

    public override void Subscribe() => SubscribeResetEvent();
    public override void Unsubscribe() => UnsubscribeResetEvent();
  

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