using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectStrength : StatusEffect
{
    public override bool triggerRecalcDamageSelf { get { return false; } } //manual call of recalc in code

    public StatusEffectStrength() : base()
    {
        OnEndTurn = null;
    }

    public override void AddFunctionToRules() => actor.dealAttackLinear.Add(DealAttackLinear);
    public override void RemoveFunctionFromRules() => actor.dealAttackLinear.Remove(DealAttackLinear);
    public int DealAttackLinear() => nrStacked;


    public override void RespondStackUpdate(int update) => CombatSystem.instance.RecalcAllCardsDamage();
}
