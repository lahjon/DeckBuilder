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

    public override void RespondStackUpdate(int update)
    {
        actor.strengthCombat = nrStacked;
        CombatSystem.instance.RecalcAllCardsDamage();
    }

}