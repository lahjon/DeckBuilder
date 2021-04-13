using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleEffectChallenge : RuleEffect
{
    public override bool isBuff { get { return false; } }
    public override bool triggerRecalcDamage { get { return true; } }

    public List<CombatActor> challengedActors = new List<CombatActor>();


    public override void OnEndTurn()
    {
        
    }

    public override void OnNewTurn()
    {
      
    }

    public override void RecieveInput(CardEffect effect)
    {
        CombatActor challenger = combatController.ActiveActor;
        if (challengedActors.Contains(challenger))
            return;

        //update the numbers
        base.RecieveInput(effect);

        if (effect.Value < 1 || challenger == actor) return;
             
        actor.dealAttackActorMods[challenger].Add(AttackEffect);
        challengedActors.Add(challenger);

        challenger.RecieveEffectNonDamageNonBlock(new CardEffect() { Type = EffectType.Challenge, Times = 1, Value = 1 });
        ((RuleEffectChallenge)challenger.effectTypeToRule[EffectType.Challenge]).challengedActors.Add(actor);
        challenger.dealAttackActorMods[actor].Add(AttackEffect);
    }

    public override void OnActorDeath()
    {
        foreach(CombatActor actor in challengedActors)
        {
            ((RuleEffectChallenge)actor.effectTypeToRule[EffectType.Challenge]).challengedActors.Remove(actor);
            actor.RecieveEffectNonDamageNonBlock(new CardEffect() { Type = EffectType.Challenge, Times = -1, Value = 1 });
        }
    }

    private float AttackEffect(float x)
    {
        return x * 2f;
    }


}
