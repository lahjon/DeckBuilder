using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectThorns : CardEffect
{
    public override bool isBuff { get { return true; } }
    public override void AddFunctionToRules()
    {
        actor.onAttackRecieved.Add(ThornIt);
    }

    public override void RemoveFunctionFromRules()
    {
        actor.onAttackRecieved.Remove(ThornIt);
    }

    public override void OnNewTurn()
    {
        
    }

    public IEnumerator ThornIt(CombatActor source)
    {
        source.TakeDamage(nrStacked);
        yield return new WaitForSeconds(0.2f);
    }
}
