    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class ConditionOptionalCardCostPaid : Condition
{
    public override void Subscribe()
    {
        base.Subscribe();
        EventManager.OnCardPlayNoArgEvent += OnEventNotification;
    }

    public override void Unsubscribe() => EventManager.OnCardPlayNoArgEvent -= OnEventNotification;
    public override bool ConditionEvaluator()
    {
        if(owner is Card card)
        {
            if (CombatSystem.instance.InProcessCard == card)
                return card.cost.optionalPaid;
            else
                return card.cost.PayableOptional();
        }
        return false;
    }

}


