using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectVulnerable : CardEffect
{
    public override bool isBuff { get { return false; } }
    public override bool triggerRecalcDamageEnemy { get { return true; } }

    public override void AddFunctionToRules()
    {
        actor.takeAttackMult.Add(VurnerableDamage);
    }

    public override void RemoveFunctionFromRules()
    {
        actor.takeAttackMult.Remove(VurnerableDamage);
    }

    public static float VurnerableDamage()
    {
        return 1.5f;
    }


}
