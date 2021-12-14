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
    }

    public override void RemoveFunctionFromRules()
    {
    }

    public override void OnActorDeath()
    {
        CombatSystem.instance.QueueEffect(this);
    }

    public IEnumerator RunEffectEnumerator()
    {
        List<CombatActor> actors = new List<CombatActor>(actor.allies);
        actors.AddRange(actor.enemies);

        foreach(CombatActor otherActor in actors)
        {
            if (otherActor != null)
            {
                otherActor.TakeDamage(actor.maxHitPoints / 2);
                yield return new WaitForSeconds(0.4f);
            }
        }
    }
}
