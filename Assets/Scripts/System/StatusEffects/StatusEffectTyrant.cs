using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectTyrant : StatusEffect
{
    public override bool triggerRecalcDamageSelf { get { return false; } } //manual call of recalc in code

    public StatusEffectTyrant() : base()
    {
        OnEndTurn = null;
        OnNewTurn = null;
    }

    public override void AddFunctionToRules()
    {
        actor.looseLifeTransform.Add(NegateDamage);
    }

    public override void RemoveFunctionFromRules()
    {
        actor.looseLifeTransform.Remove(NegateDamage);
    }

    int NegateDamage(int x)
    {
        if (actor.allies.Count == 0) return x;

        CombatActor ally = actor.allies[Random.Range(0, actor.allies.Count)];
        ally.TakeDamage(x);

        return 0;
    }

}
