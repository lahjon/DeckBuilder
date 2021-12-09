using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CombatControllerAnimatorCardAttack : CombatControllerAnimatorCard
{
    List<StatusEffectCarrier> attacks;

    CombatActor activeActor { get { return combat.ActiveActor; } }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);

        if(card.effectsOnPlay.Count != 0)
            nextState = "Effects";
        else
            nextState = "Activities & Discard";

        attacks = card.Attacks;
        if (attacks.Any())
            combat.StartCoroutine(PerformAttack());
        else
            combat.animator.Play(nextLayerState);
    }


    IEnumerator PerformAttack()
    {
        activeActor.AttackAnimation();
        foreach (StatusEffectCarrier attack in attacks)
        {
            List<CombatActor> targets = combat.GetTargets(activeActor, attack.Target, suppliedTarget);

            for (int i = 0; i < attack.Times; i++)
            {
                foreach (CombatActor actor in targets)
                {
                    int damage = RulesSystem.instance.CalculateDamage(attack.Value, activeActor, actor);
                    if (actor != null)
                        yield return combat.StartCoroutine(actor.GetAttacked(damage, activeActor));
                }
                
                if ((activeActor.hitPoints == 0 && attack.Target != CardTargetType.EnemyRandom) || !CombatSystem.instance.EnemiesInScene.Any()) break;

                if (attack.Target == CardTargetType.EnemyRandom && i != attack.Times - 1) // redraw enemy if target is random
                    targets = combat.GetTargets(activeActor, attack.Target, suppliedTarget);

                yield return new WaitForSeconds(0.1f);
            }

            if (!combat.animator.GetBool("HasWon"))
                combat.animator.Play(nextLayerState);
        }
    }

}
