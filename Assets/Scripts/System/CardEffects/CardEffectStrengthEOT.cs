using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectStrengthEOT : CardEffect
{
    public override bool isBuff { get { return true; } }
    public override bool triggerRecalcDamage { get { return true; } }

    public override void RespondStackUpdate(int update)
    {
        actor.RecieveEffectNonDamageNonBlock(new CardEffectInfo(EffectType.Strength, update, 1));
    }

    public override void OnNewTurn()
    {
    }

    public override void OnEndTurn()
    {
        RecieveInput(-nrStacked);
    }
}
