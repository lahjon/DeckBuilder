using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleEffectWeak : RuleEffect
{
    public override void AddFunctionToRules()
    {
        healthEffects.combatActor.dealAttackMods.Add(WeakDamage);
    }

    public override void RemoveFunctionFromRules()
    {
        healthEffects.combatActor.dealAttackMods.Remove(WeakDamage);
    }

    public static float WeakDamage(float x)
    {
        return x * 0.75f;
    }
}
