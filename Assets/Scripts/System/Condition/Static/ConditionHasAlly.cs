    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class ConditionHasAlly: Condition
{
    public override void Subscribe() {
        base.Subscribe();
        EventManager.OnEnemyKilledEvent += NoteEnemyKilled; 
    }
    public override void Unsubscribe() => EventManager.OnEnemyKilledEvent -= NoteEnemyKilled;

    public void NoteEnemyKilled(CombatActorEnemy data) => OnEventNotification();


    public override bool ConditionEvaluator()
    {
        Debug.Log(owner);
        Debug.Log(owner.GetOwningActor());
        Debug.Log(conditionData);
        return owner.GetOwningActor().allies.Count >= conditionData.numValue;
    }
}


