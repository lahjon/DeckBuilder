using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleEffectVurnerable : RuleEffect
{
    public override void AddFunctionToRules()
    {
        RulesSystem.instance.actorToTakeDamageMods[healthEffects.combatActor].Add(VurnerableDamage);
    }

    public override void RemoveFunctionFromRules()
    {
        RulesSystem.instance.actorToTakeDamageMods[healthEffects.combatActor].Remove(VurnerableDamage);
    }



    public float VurnerableDamage(float x)
    {
        return x * 1.5f;
    }

}
