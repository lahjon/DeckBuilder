using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectBarricade : CardEffect
{
    Func<IEnumerator> stolenFunction;
    public override bool isBuff { get { return true; } }
    public override bool stackable { get { return false; } }

    public CardEffectBarricade() : base()
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
