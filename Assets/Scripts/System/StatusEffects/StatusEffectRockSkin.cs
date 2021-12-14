using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectRockSkin : StatusEffect
{
    public override bool triggerRecalcDamageSelf => false;
    public override bool stackable => true; 

    public StatusEffectRockSkin() : base()
    {
        OnEndTurn = null;
        OnNewTurn = null;
    }

    public override void AddFunctionToRules()
    {
        actor.onAttackRecieved.Add(BlockMeDaddy);
    }

    public override void RemoveFunctionFromRules()
    {
        actor.onAttackRecieved.Remove(BlockMeDaddy);
    }

    public IEnumerator BlockMeDaddy(CombatActor source)
    {
        if(actor.GetBlock() == 0 && actor.hitPoints > 0)
            yield return CombatSystem.instance.StartCoroutine(actor.GainBlock(nrStacked));
    }



}
