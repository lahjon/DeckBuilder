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
        return
            WorldStateSystem.instance.currentWorldState == WorldState.Combat &&
            CombatSystem.instance.InProcessCard is Card card ? card.cost.optionalPaid : false;
    }

}


