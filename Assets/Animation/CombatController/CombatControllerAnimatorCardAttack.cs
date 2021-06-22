using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllerAnimatorCardAttack : CombatControllerAnimatorCard
{
    CardEffectInfo attack;

    CombatActor activeActor { get { return combatController.ActiveActor; } }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);

        if(card.effectsOnPlay.Count != 0)
            nextState = "Effects";
        else
            nextState = "Activities & Discard";


        attack = card.Damage;
        if (attack.Value == 0)
            combatController.animator.Play(nextLayerState);
        else
            combatController.StartCoroutine(PerformAttack());
    }



    IEnumerator PerformAttack()
    {
        List<CombatActor> targets = combatController.GetTargets(activeActor, attack, suppliedTarget);

        for(int i = 0; i < attack.Times; i++)
        {
            foreach (CombatActor actor in targets)
            {
                int damage = RulesSystem.instance.CalculateDamage(attack.Value, activeActor, actor);
                yield return combatController.StartCoroutine(actor.GetAttacked(damage, activeActor));
            }

            if (attack.Target == CardTargetType.EnemyRandom && i != attack.Times -1) // redraw enemy if target is random
                targets = combatController.GetTargets(activeActor, attack, suppliedTarget);
        }

        combatController.animator.Play(nextLayerState);
    }
        



}
