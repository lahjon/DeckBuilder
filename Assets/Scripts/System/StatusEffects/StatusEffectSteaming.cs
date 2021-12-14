using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectSteaming : StatusEffect
{
    public override bool triggerRecalcDamageSelf { get { return false; } }

    public StatusEffectSteaming() : base()
    {
        OnEndTurn = null;
        OnNewTurn = _OnNewTurn;
    }

    public override void AddFunctionToRules()
    {
        actor.onAttackRecieved.Add(IncreaseTheAnger);
    }

    public override void RemoveFunctionFromRules()
    {
        actor.onAttackRecieved.Remove(IncreaseTheAnger);
    }

    protected override IEnumerator _OnNewTurn()
    {
        yield return CombatSystem.instance.StartCoroutine(RecieveInput(-nrStacked));
    }

    public IEnumerator IncreaseTheAnger(CombatActor source)
    {
        CombatSystem.instance.ModifyEnergy(EnergyType.Rage, nrStacked);
        yield return new WaitForSeconds(0.3f);
    }



}