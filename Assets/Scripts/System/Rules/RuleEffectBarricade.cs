using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleEffectBarricade : RuleEffect
{
    Func<IEnumerator> stolenFunction;
    public override bool isBuff { get { return true; } }

    public override void AddFunctionToRules()
    {
        stackable = false;
        stolenFunction = actor.RemoveAllBlock;
        actor.actionsNewTurn.Remove(actor.RemoveAllBlock);
    }

    public override void RemoveFunctionFromRules()
    {
        actor.actionsNewTurn.Add(stolenFunction);
    }

    public override void OnNewTurnBehaviour()
    {
        
    }


}
