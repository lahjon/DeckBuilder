using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectChallenge : StatusEffect
{
    public override bool triggerRecalcDamageSelf { get { return true; } }

    public List<CombatActor> challengedActors = new List<CombatActor>();

    public StatusEffectChallenge() : base()
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
                    challenger.RecieveEffectNonDamage(new StatusEffectCarrier(StatusEffectType.Challenge,1))
                    );
                ((StatusEffectChallenge)challenger.effectTypeToRule[StatusEffectType.Challenge]).challengedActors.Add(actor);
                challenger.dealAttackActorMods[actor].Add(AttackEffect);
            }

            actor.RecalcDamage();
        }
    }

    public override void OnActorDeath()
    {
        foreach(CombatActor actor in challengedActors)
        {
            ((StatusEffectChallenge)actor.effectTypeToRule[StatusEffectType.Challenge]).challengedActors.Remove(actor);
            CombatSystem.instance.StartCoroutine(actor.RecieveEffectNonDamage(new StatusEffectCarrier(StatusEffectType.Challenge, -1)));
        }
    }

    private float AttackEffect()
    {
        return 2f;
    }


}
