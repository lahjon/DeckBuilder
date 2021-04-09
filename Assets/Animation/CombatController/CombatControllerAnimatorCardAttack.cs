using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllerAnimatorCardAttack : CombatControllerAnimatorCard
{
    CardEffect attack;
    (CardEffect effect, List<CombatActor> targets) effectAndTarget;

    CombatActor activeActor { get { return combatController.ActiveActor; } }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);

        if(card.Effects.Count != 0)
            nextState = "Effects";
        else
            nextState = "Activities & Discard";


        attack = card.Damage;
        if (attack.Value == 0)
            combatController.animator.Play(nextLayerState);
        else
        {
            effectAndTarget = combatController.GetTargets(activeActor, attack, suppliedTarget);
            combatController.StartCoroutine(PerformAttack());
        }
    }



    IEnumerator PerformAttack()
    {
        for(int i = 0; i < effectAndTarget.effect.Times; i++)
        {
            foreach (CombatActor actor in effectAndTarget.targets)
            {
                int damage = RulesSystem.instance.CalculateDamage(effectAndTarget.effect.Value, activeActor, actor);
                yield return combatController.StartCoroutine(actor.GetAttacked(damage, activeActor));
            }
        }

        combatController.animator.Play(nextLayerState);
    }
        



}
