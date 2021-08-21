    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Condition : IEventSubscriber
{
    public ConditionStruct conditionStruct;
    public bool value;
    public Func<ConditionStruct, bool> ConditionEvaluator;

    public Action OnPreConditionUpdate;
    public Action OnConditionFlipTrue;
    public Action OnConditionFlipFalse;
    public Action OnConditionFlip;

    public Condition(ConditionStruct conditionStruct, Action OnPreConditionUpdate = null, Action OnConditionFlip = null, Action OnConditionFlipTrue = null, Action OnConditionFlipFalse = null)
    {
        this.conditionStruct = conditionStruct;
        if (conditionStruct.type == ConditionType.None)
        {
            value = true;
            return;
        }

        ConditionEvaluator = ConditionSystem.GetConditionChecker(conditionStruct.type);
        this.OnPreConditionUpdate = OnPreConditionUpdate;
        this.OnConditionFlip = OnConditionFlip;
        this.OnConditionFlipTrue = OnConditionFlipTrue;
        this.OnConditionFlipFalse = OnConditionFlipFalse;
        Subscribe();
    }

    public Condition()
    {
        conditionStruct = new ConditionStruct() { type = ConditionType.None };
        value = true;
    }


    public void Subscribe()
    {
        //Debug.Log("Subbing");
        switch (conditionStruct.type)
        {
            case ConditionType.None:
                return;
            case ConditionType.CardsPlayedAtLeast:
            case ConditionType.CardsPlayedAtMost:
            case ConditionType.LastCardPlayedTurnType:
                EventManager.OnCardPlayNoArgEvent       += OnEventNotification;
                break;
            case ConditionType.WinCombat:
                EventManager.OnCombatWonEvent           += OnEventNotification;
                break;
            case ConditionType.ClearTile:
                EventManager.OnCompleteTileEvent        += OnEventNotification;
                break;
            case ConditionType.KillEnemy:
                EventManager.OnEnemyKilledEvent         += OnEventNotification;
                break;

            default:
                break;
        }
        OnEventNotification();
    }

    public void Unsubscribe()
    {
        switch (conditionStruct.type)
        {
            case ConditionType.None:
                return;
            case ConditionType.CardsPlayedAtLeast:
            case ConditionType.CardsPlayedAtMost:
            case ConditionType.LastCardPlayedTurnType:
                EventManager.OnCardPlayNoArgEvent       -= OnEventNotification;
                break;
            case ConditionType.WinCombat:
                EventManager.OnCombatWonEvent           -= OnEventNotification;
                break;
            case ConditionType.ClearTile:
                EventManager.OnCompleteTileEvent        -= OnEventNotification;
                break;
            case ConditionType.KillEnemy:
                EventManager.OnEnemyKilledEvent         -= OnEventNotification;
                break;

            default:
                break;
        }
    }
    public virtual void OnEventNotification()
    {
        if (ConditionEvaluator == null) return;
        bool newVal = ConditionEvaluator(conditionStruct);

        OnPreConditionUpdate?.Invoke();

        if (newVal != value)
        {
            value = newVal;
            OnConditionFlip?.Invoke();
            if (newVal)
                OnConditionFlipTrue?.Invoke();
            else
                OnConditionFlipFalse?.Invoke();
        }
    }

    public virtual void OnEventNotification(EnemyData enemy)
    {
        OnEventNotification();
    }

}


