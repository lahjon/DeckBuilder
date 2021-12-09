using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectBarrier : CardEffect
{
    public override bool triggerRecalcDamageSelf { get { return false; } } //manual call of recalc in code

    public CardEffectBarrier() : base()
    {
        OnEndTurn = null;
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
        CombatSystem.instance.StartCoroutine(RecieveInput(-1));
        return 0;
    }

}
