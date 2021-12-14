using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectVolatile : StatusEffect, ICombatEffect
{
    public StatusEffectVolatile() : base()
    {
        OnEndTurn = null;
        OnNewTurn = _OnNewTurn;
    }

    public override void AddFunctionToRules()
    {
    }

    public override void RemoveFunctionFromRules()
    {
    }

    protected override IEnumerator _OnNewTurn()
    {
        yield return CombatSystem.instance.StartCoroutine(RecieveInput(-1));
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
                otherActor.TakeDamage((actor.maxHitPoints+1) / 2);
                yield return new WaitForSeconds(0.3f);
            }
        }
    }
}
