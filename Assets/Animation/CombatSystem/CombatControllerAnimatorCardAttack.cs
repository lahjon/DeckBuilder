using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllerAnimatorCardAttack : CombatControllerAnimatorCard
{
    CardEffectCarrier attack;

    CombatActor activeActor { get { return combat.ActiveActor; } }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);

        if(card.effectsOnPlay.Count != 0)
            nextState = "Effects";
        else
            nextState = "Activities & Discard";

        attack = card.Damage;
        if (attack.Value == 0)
            combat.animator.Play(nextLayerState);
        else
            combat.StartCoroutine(PerformAttack());
    }



    IEnumerator PerformAttack()
    {
        List<CombatActor> targets = combat.GetTargets(activeActor, attack.Target, suppliedTarget);

        for(int i = 0; i < attack.Times; i++)
        {
            foreach (CombatActor actor in targets)
            {
                int damage = RulesSystem.instance.CalculateDamage(attack.Value, activeActor, actor);
                if(actor != null && actor.hitPoints > 0)
                    yield return combat.StartCoroutine(actor.GetAttacked(damage, activeActor));
            }

            if (attack.Target == CardTargetType.EnemyRandom && i != attack.Times -1) // redraw enemy if target is random
                targets = combat.GetTargets(activeActor, attack.Target, suppliedTarget);

            if (activeActor.hitPoints == 0) break;
            yield return new WaitForSeconds(0.1f);
        }

        Debug.Log("leaving perform attack. Has won is:  " + combat.animator.GetBool("HasWon"));
        if (!combat.animator.GetBool("HasWon"))
            combat.animator.Play(nextLayerState);
    }
        



}
