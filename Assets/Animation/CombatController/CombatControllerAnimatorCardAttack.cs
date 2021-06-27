using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllerAnimatorCardAttack : CombatControllerAnimatorCard
{
    CardEffectInfo attack;
    (CardEffectInfo effect, List<CombatActor> targets) effectAndTarget;

    CombatActor activeActor { get { return CombatSystem.instance.ActiveActor; } }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);

        if(card.effectsOnPlay.Count != 0)
            nextState = "Effects";
        else
            nextState = "Activities & Discard";


        attack = card.Damage;
        if (attack.Value == 0)
            CombatSystem.instance.animator.Play(nextLayerState);
        else
        {
            effectAndTarget = CombatSystem.instance.GetTargets(activeActor, attack, suppliedTarget);
            CombatSystem.instance.StartCoroutine(PerformAttack());
        }
    }



    IEnumerator PerformAttack()
    {
        for(int i = 0; i < effectAndTarget.effect.Times; i++)
        {
            foreach (CombatActor actor in effectAndTarget.targets)
            {
                int damage = RulesSystem.instance.CalculateDamage(effectAndTarget.effect.Value, activeActor, actor);
                yield return CombatSystem.instance.StartCoroutine(actor.GetAttacked(damage, activeActor));
            }
        }

        CombatSystem.instance.animator.Play(nextLayerState);
    }
        



}
