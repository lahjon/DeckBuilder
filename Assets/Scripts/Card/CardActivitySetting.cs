using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[System.Serializable]
public class CardActivitySetting : ICardTextElement
{
    public CardActivityType type;
    public string parameter;
    public Condition condition;

    public CardActivitySetting(CardActivityData data, Card card, Action OnPreConditionUpdate = null)
    {
        type = data.type;
        parameter = data.parameter;

        condition = new Condition(data.conditionStruct, OnPreConditionUpdate);

        if (data.conditionStruct.type != ConditionType.None)
        {
            card.registeredConditions.Add(condition);
            card.registeredSubscribers.Add(condition);
        }
    }

    public string GetElementText()
    {
        return CardActivitySystem.instance.DescriptionByCardActivity(this);
    }

    public string GetElementToolTip()
    {
        return CardActivitySystem.instance.ToolTipByCardActivity(this);
    }
}



