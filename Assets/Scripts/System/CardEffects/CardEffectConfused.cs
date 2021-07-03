using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectConfused : CardEffect
{
    Func<IEnumerator> stolenFunction;
    public override bool isBuff { get { return true; } }
    public override bool stackable { get { return false; } }


    public override void AddFunctionToRules()
    {
        actor.targetDistorter[CardTargetType.EnemySingle] = CardTargetType.EnemyRandom;
    }

    public override void RemoveFunctionFromRules()
    {
        //TODO G�r en stack om det finns fler som vill skriva p� denna
        actor.targetDistorter[CardTargetType.EnemySingle] = CardTargetType.EnemySingle;
    }

}