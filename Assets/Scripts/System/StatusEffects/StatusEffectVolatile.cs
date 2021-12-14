using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectVolatile : StatusEffect, ICombatEffect
{
    public StatusEffectVolatile() : base()
    {
        OnNewTurn = null;
    }

    public override void AddFunctionToRules()
    {
        EventManager.OnEnemyKilledEvent += QueueMeUp;
    }

    public override void RemoveFunctionFromRules()
    {
        EventManager.OnEnemyKilledEvent -= QueueMeUp;
    }

    public void QueueMeUp(CombatActorEnemy enemy)
    {

    }

    public IEnumerator RunEffectEnumerator()
    {
        yield return null;
    }
}
