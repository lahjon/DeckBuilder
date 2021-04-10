using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleEffectWeak : RuleEffect
{
    public override bool isBuff { get { return false; } }
    public override void AddFunctionToRules()
    {
        actor.dealAttackMods.Add(WeakDamage);
    }

    public override void RemoveFunctionFromRules()
    {
        actor.dealAttackMods.Remove(WeakDamage);
    }

    public static float WeakDamage(float x)
    {
        return x * 0.75f;
    }
}
