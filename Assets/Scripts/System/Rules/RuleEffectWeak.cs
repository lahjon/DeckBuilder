using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleEffectWeak : RuleEffect
{
    public override void AddFunctionToRules()
    {
        RulesSystem.instance.actorToGiveAttackMods[healthEffects.combatActor].Add(WeakDamage);
    }

    public override void RemoveFunctionFromRules()
    {
        RulesSystem.instance.actorToGiveAttackMods[healthEffects.combatActor].Remove(WeakDamage);
    }



    public float WeakDamage(float x)
    {
        return x * 0.75f;
    }
}
