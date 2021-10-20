using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[System.Serializable]
public class CardActivitySetting : ICardTextElement
{
    public CardActivityType type;
    public string strParameter;
    public int val;
    public Condition condition;

    public CardActivitySetting(CardActivityData data, Card card = null, Action OnPreConditionUpdate = null)
    {
        type = data.type;
        strParameter = data.strParameter;
        val = data.val;

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

    public bool CanAbsorb(CardActivityData data)
    {
        return      type == data.type && 
                    strParameter == data.strParameter && 
                    condition.conditionData.type == data.conditionStruct.type; 
    }

    public void AbsorbModifierData(CardActivityData data)
    {
        val += data.val;
    }
}



