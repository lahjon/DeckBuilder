using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllerAnimatorCardAttack : CombatControllerAnimatorCard
{
    CardEffectInfo attack;

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
            CombatSystem.instance.StartCoroutine(PerformAttack());
    }



    IEnumerator PerformAttack()
    {
        List<CombatActor> targets = CombatSystem.instance.GetTargets(activeActor, attack.Target, suppliedTarget);

        for(int i = 0; i < attack.Times; i++)
        {
            foreach (CombatActor actor in targets)
            {
                int damage = RulesSystem.instance.CalculateDamage(attack.Value, activeActor, actor);
                yield return CombatSystem.instance.StartCoroutine(actor.GetAttacked(damage, activeActor));
            }

            if (attack.Target == CardTargetType.EnemyRandom && i != attack.Times -1) // redraw enemy if target is random
                targets = CombatSystem.instance.GetTargets(activeActor, attack.Target, suppliedTarget);
        }

        CombatSystem.instance.animator.Play(nextLayerState);
    }
        



}
