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

    public CardCombat card;

    public CardCondition(Card card, ConditionStruct conditionStruct)
    {
        this.conditionStruct = conditionStruct;
        if (conditionStruct.type != ConditionType.None)
            CheckConditionAction = ConditionSystem.GetConditionChecker(conditionStruct.type);
        else
            value = true;

        if (card is CardCombat cc) this.card = cc;
    }

    public CardCondition()
    {
        conditionStruct = new ConditionStruct() { type = ConditionType.None };
        value = true;
    }

    public void CheckCondition()
    {
        bool newVal = CheckConditionAction(conditionStruct.value);
        if (newVal != value) {
            value = newVal;
            if(card != null) card.EvaluateHighlightNotSelected();
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

