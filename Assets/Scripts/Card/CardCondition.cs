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

    public Func<string, bool> CheckConditionAction;

    public CardCondition(ConditionStruct conditionStruct)
    {
        this.conditionStruct = conditionStruct;
        if (conditionStruct.type != ConditionType.None)
            CheckConditionAction = ConditionSystem.GetConditionChecker(conditionStruct.type);
        else
            value = true;
    }

    public CardCondition()
    {
        conditionStruct = new ConditionStruct() { type = ConditionType.None };
        value = true;
    }

    public void CheckCondition()
    {
        Debug.Log("Checking condition type and val:" + conditionStruct.type + "," + conditionStruct.value);
        value = CheckConditionAction(conditionStruct.value);
    }

    public void Subscribe()
    {
        Debug.Log("Subbing");
        switch (conditionStruct.type)
        {
            case ConditionType.None:
                return;
            case ConditionType.CardsPlayedAbove:
            case ConditionType.CardsPlayedBelow:
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
            case ConditionType.CardsPlayedAbove:
            case ConditionType.CardsPlayedBelow:
            case ConditionType.LastCardPlayedTurnType:
                EventManager.OnCardPlayNoArgEvent -= CheckCondition;
                break;
            case ConditionType.KillEnemy:
                EventManager.OnEnemyKilledNoArgEvent -= CheckCondition;
                break;
        }
    }
}

