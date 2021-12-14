using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ConditionCountingWinCombat : ConditionCounting
{
    public override void Subscribe() {
        base.Subscribe();
        EventManager.OnCombatWonEvent += OnEventNotification; 
    }

    public override void Unsubscribe()
    {
        base.Unsubscribe();
        EventManager.OnCombatWonEvent -= OnEventNotification;
    }


}