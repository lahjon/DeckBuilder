using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[System.Serializable]
public class CombatActivitySetting : ICardTextElement, ICardUpgradableComponent 
{
    public CombatActivityType type;
    public string strParameter;
    public int val;
    public Condition condition;

    HashSet<ModifierType> modifiedTypes = new HashSet<ModifierType>();
    public CombatActivitySetting(CardActivityData data, Card card = null, Action OnPreConditionUpdate = null)
    {
        type = data.type;
        strParameter = data.strParameter;
        val = data.val;

        condition = Condition.Factory(data.conditionStruct, card, OnPreConditionUpdate);

        if (data.conditionStruct != null && data.conditionStruct.type != ConditionType.None)
        {
            card.registeredConditions.Add(condition);
            card.registeredSubscribers.Add(condition);
        }
    }

    public string GetElementText()
    {
        string retString = "";

        foreach (ModifierType mod in modifiedTypes)
            retString += Helpers.ModifierToIconName[mod];
        return retString + CombatActivitySystem.instance.DescriptionByCardActivity(this);
    }

    public string GetElementToolTip()
    {
        return CombatActivitySystem.instance.ToolTipByCardActivity(this);
    }

    public bool CanAbsorb<T>(T modifier)
    {
        if(modifier is CardActivityData data)
        {
            return type == data.type &&
                   strParameter == data.strParameter &&
                   condition.GetCondType() == data.conditionStruct.type;
        }
        return false;
    }

    public void AbsorbModifier<T>(T modifier)
    {
        if (modifier is CardActivityData data)
            val += data.val;
    }

    public void RegisterModified(ModifierType type)
    {
        if(type != ModifierType.None)
            modifiedTypes.Add(type);
    }
}



