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
        if (owner.GetOwningActor() == CombatSystem.instance.Hero)
            return 100 * WorldSystem.instance.characterManager.CurrentHealth / CharacterStats.Health < conditionData.numValue;
        else
            return 100 * owner.GetOwningActor().hitPoints / owner.GetOwningActor().maxHitPoints < conditionData.numValue;

    }
    
}


