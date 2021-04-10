using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleEffectChallenge : RuleEffect
{
    public override bool isBuff { get { return false; } }
    public override bool triggerRecalcDamage { get { return true; } }

    List<CombatActor> ChallengedActors = new List<CombatActor>();

    public override void AddFunctionToRules()
    {

    }

    public override void RemoveFunctionFromRules()
    {

    }


}
