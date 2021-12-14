using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectDazed : StatusEffect
{
    public override bool triggerRecalcDamageEnemy { get { return false; } }

    public override void AddFunctionToRules()
    {
        actor.gainBlockMult.Add(BlockMultiplier);
        actor.OnBlockGained.Add(RemoveOnGain);
    }

    public override void RemoveFunctionFromRules()
    {
        actor.gainBlockMult.Remove(BlockMultiplier);
        actor.OnBlockGained.Remove(RemoveOnGain);
    }

    public static float BlockMultiplier()
    {
        return 0f;
    }

    protected override IEnumerator _OnEndTurn()
    {
        yield return CombatSystem.instance.StartCoroutine(actor.RecieveEffectNonDamage(new StatusEffectCarrier(StatusEffectType.Dazed, -nrStacked)));
    }

    public IEnumerator RemoveOnGain()
    {
        yield return CombatSystem.instance.StartCoroutine(actor.RecieveEffectNonDamage(new StatusEffectCarrier(StatusEffectType.Dazed, -1)));
    }



}
