using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ConditionCountingSpendEnergySpecific : ConditionCounting
{
    public override void Subscribe() {
        base.Subscribe();
        EventManager.OnEnergyInfoChangedEvent += CheckValid; 
    }

    public override void Unsubscribe()
    {
        base.Unsubscribe();
        EventManager.OnEnergyInfoChangedEvent -= CheckValid;
    }

    public void CheckValid(Dictionary<EnergyType, int> changes)
    {
        if (string.IsNullOrEmpty(conditionData.strParameter))
            OnEventNotification();
        else
        {
            EnergyType type = conditionData.strParameter.ToEnum<EnergyType>();
            if (changes.ContainsKey(type) && changes[type] >= 0)
                OnEventNotification();
        }
    }

}