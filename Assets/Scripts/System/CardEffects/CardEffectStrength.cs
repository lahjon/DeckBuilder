using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectStrength : CardEffect
{
    public override bool isBuff { get { return true; } }
    public override bool triggerRecalcDamage { get { return false; } } //manual call of recalc in code

    public CardEffectStrength() : base()
    {
        OnEndTurn = null;
    }

    public override void RespondStackUpdate(int update)
    {
        actor.strengthCombat = nrStacked;
        CombatSystem.instance.RecalcAllCardsDamage();
    }

}
