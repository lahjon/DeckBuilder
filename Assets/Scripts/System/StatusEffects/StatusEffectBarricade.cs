using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectBarricade : StatusEffect
{
    Func<IEnumerator> stolenFunction;
    public override bool stackable { get { return false; } }

    public StatusEffectBarricade() : base()
    {
        OnEndTurn = null;
    }

    public override void AddFunctionToRules()
    {
        stolenFunction = actor.RemoveAllBlock;
        actor.actionsNewTurn.Remove(actor.RemoveAllBlock);
    }

    public override void RemoveFunctionFromRules()
    {
        actor.actionsNewTurn.Add(stolenFunction);
    }

}
