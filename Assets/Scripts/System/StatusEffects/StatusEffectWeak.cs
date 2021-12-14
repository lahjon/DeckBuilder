using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectWeak : StatusEffect
{
    public override bool triggerRecalcDamageSelf { get { return true; } }
    public override void AddFunctionToRules()
    {
        actor.dealAttackMult.Add(WeakDamage);
    }

    public override void RemoveFunctionFromRules()
    {
        actor.dealAttackMult.Remove(WeakDamage);
    }

    public static float WeakDamage()
    {
        return 0.75f;
    }
}