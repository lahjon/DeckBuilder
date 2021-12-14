using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectSpiked : StatusEffect
{
    public override bool triggerRecalcDamageSelf { get { return false; } }

    public StatusEffectSpiked() : base()
    {
        OnEndTurn = null;
        OnNewTurn = _OnNewTurn;
    }

    public override void AddFunctionToRules()
    {
        actor.onAttackRecieved.Add(SpikeIt);
    }

    public override void RemoveFunctionFromRules()
    {
        actor.onAttackRecieved.Remove(SpikeIt);
    }

    protected override IEnumerator _OnNewTurn()
    {
        yield return CombatSystem.instance.StartCoroutine(actor.RecieveEffectNonDamage(new StatusEffectCarrier(StatusEffectType.Spiked, -nrStacked)));
    }

    public IEnumerator SpikeIt(CombatActor source)
    {
        source.TakeDamage(nrStacked);
        yield return new WaitForSeconds(0.3f);
    }



}
