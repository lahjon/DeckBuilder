using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


[System.Serializable]
public class CardCondition : IEvents
{
    public ConditionStruct conditionStruct;
    public bool value;

    public Func<string, bool> ConditionEvaluator;

    Action OnPreConditionUpdate;
    Action OnConditionFlipTrue;
    Action OnConditionFlipFalse;
    Action OnConditionFlip;

    public CardCondition(ConditionStruct conditionStruct, Action OnPreConditionUpdate = null, Action OnConditionFlip = null, Action OnConditionFlipTrue = null, Action OnConditionFlipFalse = null)
    {
        this.conditionStruct = conditionStruct;
        if (conditionStruct.type != ConditionType.None)
            ConditionEvaluator = ConditionSystem.GetConditionChecker(conditionStruct.type);
        else
            value = true;

        this.OnPreConditionUpdate   = OnPreConditionUpdate;
        this.OnConditionFlip        = OnConditionFlip;
        this.OnConditionFlipTrue    = OnConditionFlipTrue;
        this.OnConditionFlipFalse   = OnConditionFlipFalse;
    }

    public CardCondition()
    {
        conditionStruct = new ConditionStruct() { type = ConditionType.None };
        value = true;
    }

    public void CheckCondition()
    {
        bool newVal = ConditionEvaluator(conditionStruct.value);

        OnPreConditionUpdate?.Invoke();
        if (newVal != value) {
            value = newVal;
            OnConditionFlip?.Invoke();
            if (newVal)
                OnConditionFlipTrue?.Invoke();
            else
                OnConditionFlipFalse?.Invoke();
        }
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
                EventManager.OnCardPlayNoArgEvent += CheckCondition;
                break;
            case ConditionType.KillEnemy:
                EventManager.OnEnemyKilledNoArgEvent += CheckCondition;
                break;
        }
        CheckCondition();
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
                EventManager.OnCardPlayNoArgEvent -= CheckCondition;
                break;
            case ConditionType.KillEnemy:
                EventManager.OnEnemyKilledNoArgEvent -= CheckCondition;
                break;
        }
    }
}

