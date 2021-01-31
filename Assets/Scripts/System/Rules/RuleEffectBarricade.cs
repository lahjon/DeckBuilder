using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleEffectBarricade : RuleEffect
{
    new bool stackable = false;
    Func<CombatActor,IEnumerator> stolenFunction;
   

    public override void AddFunctionToRules()
    {
        stolenFunction = RulesSystem.instance.RemoveAllBlock;
        RulesSystem.instance.ActorToStartTurn[healthEffects.combatActor].Remove(stolenFunction);
    }

    public override void RemoveFunctionFromRules()
    {
        RulesSystem.instance.ActorToStartTurn[healthEffects.combatActor].Add(stolenFunction);
    }

    public override void OnNewTurnBehaviour()
    {
        
    }


}
