using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CardActivitySetting : ICardTextElement
{
    public CardActivityType type;
    public string parameter;

    public ConditionStruct conditionStruct;

    public CardComponentExecType execTime;
    public string GetElementText()
    {
        return CardActivitySystem.instance.DescriptionByCardActivity(this);
    }

    public string GetElementToolTip()
    {
        return CardActivitySystem.instance.ToolTipByCardActivity(this);
    }
}



