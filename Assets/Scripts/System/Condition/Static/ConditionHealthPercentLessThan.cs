    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class ConditionHealthPercentLessThan : Condition
{
    public override void Subscribe()
    {
        base.Subscribe();
        EventManager.OnHealthChangedEventnoArg += OnEventNotification;
    }

    public override void Unsubscribe() => EventManager.OnHealthChangedEventnoArg -= OnEventNotification;
    public override bool ConditionEvaluator()
    {
        return 100 * WorldSystem.instance.characterManager.CurrentHealth / CharacterStats.Health < conditionData.numValue;
    }
    
}


