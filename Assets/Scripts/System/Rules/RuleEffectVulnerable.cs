using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleEffectVulnerable : RuleEffect
{
    public override bool isBuff { get { return false; } }
    public override bool triggerRecalcDamage { get { return true; } }


    public override void AddFunctionToRules()
    {
        actor.takeAttackMods.Add(VurnerableDamage);
    }

    public override void RemoveFunctionFromRules()
    {
        actor.takeAttackMods.Remove(VurnerableDamage);
    }

    public static float VurnerableDamage(float x)
    {
        return x * 1.5f;
    }


}