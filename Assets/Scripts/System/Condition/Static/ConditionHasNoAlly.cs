    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class ConditionHasNoAlly: Condition
{
    public override void Subscribe() {
        base.Subscribe();
        EventManager.OnEnemyKilledEvent += NoteEnemyKilled; 
    }
    public override void Unsubscribe() => EventManager.OnEnemyKilledEvent -= NoteEnemyKilled;

    public void NoteEnemyKilled(CombatActor data) => OnEventNotification();

    public override bool ConditionEvaluator() => owner.GetOwningActor().allies.Count == 0;
}


