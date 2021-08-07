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
    private bool _value;
    public bool value { get { return conditionStruct.type == ConditionType.None || _value == true; } }

    public void CheckCondition()
    {
        Debug.Log("Checking Condition");
        _value = ConditionSystem.CheckCondition(conditionStruct);
    }

    public void Subscribe()
    {
        Debug.Log("Subbing");
        switch (conditionStruct.type)
        {
            case ConditionType.CardsPlayedAbove:
            case ConditionType.CardsPlayedBelow:
                EventManager.OnCardPlayNoArgEvent += CheckCondition;
                break;
            case ConditionType.KillEnemy:
                EventManager.OnEnemyKilledNoArgEvent += CheckCondition;
                break;
        }
    }

    public void Unsubscribe()
    {
        switch (conditionStruct.type)
        {
            case ConditionType.CardsPlayedAbove:
            case ConditionType.CardsPlayedBelow:
                EventManager.OnCardPlayNoArgEvent -= CheckCondition;
                break;
            case ConditionType.KillEnemy:
                EventManager.OnEnemyKilledNoArgEvent -= CheckCondition;
                break;
        }
    }
}

