using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectStrength : CardEffect
{
    public override bool isBuff { get { return true; } }
    public override bool triggerRecalcDamage { get { return true; } }
    
    
    public override void RecieveInput(CardEffectInfo effect)
    {
        if (effect.Times == 0 || effect.Value == 0) return;

        nrStacked += effect.Times * effect.Value;

        actor.healthEffectsUI.UpdateEffectUI(this);

        actor.strengthCombat = nrStacked;

        combatController.RecalcAllCardsDamage();

        if (nrStacked == 0) Dismantle();
    }

    public override void OnNewTurn()
    {
    }
}
