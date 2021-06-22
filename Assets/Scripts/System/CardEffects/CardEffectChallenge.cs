using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectChallenge : CardEffect
{
    public override bool isBuff { get { return false; } }
    public override bool triggerRecalcDamage { get { return true; } }

    public List<CombatActor> challengedActors = new List<CombatActor>();

    public CardEffectChallenge() : base()
    {
        OnNewTurn = null;
    }

    public override IEnumerator RecieveInput(int stackUpdate)
    {
        CombatActor challenger = combatController.ActiveActor;
        if (!challengedActors.Contains(challenger))
        {
            //update the numbers
            yield return combatController.StartCoroutine(base.RecieveInput(stackUpdate));

            if (!(stackUpdate < 1 || challenger == actor))
            {
                actor.dealAttackActorMods[challenger].Add(AttackEffect);
                challengedActors.Add(challenger);

                yield return combatController.StartCoroutine(
                    challenger.RecieveEffectNonDamageNonBlock(new CardEffectInfo() { Type = EffectType.Challenge, Times = 1, Value = 1 })
                    );
                ((CardEffectChallenge)challenger.effectTypeToRule[EffectType.Challenge]).challengedActors.Add(actor);
                challenger.dealAttackActorMods[actor].Add(AttackEffect);
            }
        }
    }

    public override void OnActorDeath()
    {
        foreach(CombatActor actor in challengedActors)
        {
            ((CardEffectChallenge)actor.effectTypeToRule[EffectType.Challenge]).challengedActors.Remove(actor);
            combatController.StartCoroutine(actor.RecieveEffectNonDamageNonBlock(new CardEffectInfo() { Type = EffectType.Challenge, Times = -1, Value = 1 }));
        }
    }

    private float AttackEffect()
    {
        return 2f;
    }


}
