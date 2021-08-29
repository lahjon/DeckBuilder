using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectEnvenom : CardEffect
{
    public override bool isBuff { get { return true; } }

    public CardEffectEnvenom() : base()
    {
        OnEndTurn = null;
    }

    public override void AddFunctionToRules()
    {
        actor.onUnblockedDmgDealt.Add(FeelThePoisonDagger);
    }

    public override void RemoveFunctionFromRules()
    {
        actor.onUnblockedDmgDealt.Remove(FeelThePoisonDagger);
    }

    IEnumerator FeelThePoisonDagger(CombatActor hurtEnemy)
    {
        yield return CombatSystem.instance.StartCoroutine(hurtEnemy.RecieveEffectNonDamageNonBlock(new CardEffectCarrier(EffectType.Poison,nrStacked)));
    }

}
