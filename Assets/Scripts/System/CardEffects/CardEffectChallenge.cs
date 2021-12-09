using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectChallenge : CardEffect
{
    public override bool triggerRecalcDamageSelf { get { return true; } }

    public List<CombatActor> challengedActors = new List<CombatActor>();

    public CardEffectChallenge() : base()
    {
        OnEndTurn = null;
    }

    public override IEnumerator RecieveInput(int stackUpdate)
    {
        CombatActor challenger = CombatSystem.instance.ActiveActor;
        if (!challengedActors.Contains(challenger))
        {
            //update the numbers
            yield return CombatSystem.instance.StartCoroutine(base.RecieveInput(stackUpdate));

            if (!(stackUpdate < 1 || challenger == actor))
            {
                actor.dealAttackActorMods[challenger].Add(AttackEffect);
                challengedActors.Add(challenger);

                yield return CombatSystem.instance.StartCoroutine(
                    challenger.RecieveEffectNonDamageNonBlock(new CardEffectCarrier(StatusEffectType.Challenge,1))
                    );
                ((CardEffectChallenge)challenger.effectTypeToRule[StatusEffectType.Challenge]).challengedActors.Add(actor);
                challenger.dealAttackActorMods[actor].Add(AttackEffect);
            }

            actor.RecalcDamage();
        }
    }

    public override void OnActorDeath()
    {
        foreach(CombatActor actor in challengedActors)
        {
            ((CardEffectChallenge)actor.effectTypeToRule[StatusEffectType.Challenge]).challengedActors.Remove(actor);
            CombatSystem.instance.StartCoroutine(actor.RecieveEffectNonDamageNonBlock(new CardEffectCarrier(StatusEffectType.Challenge, -1)));
        }
    }

    private float AttackEffect()
    {
        return 2f;
    }


}
