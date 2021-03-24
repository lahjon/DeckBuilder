using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllerAnimatorCardAttack : CombatControllerAnimatorCard
{
    CardEffect attack;
    List<CombatActor> targetActors = new List<CombatActor>();

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
            targetActors.Clear();
            targetActors = combatController.GetTargets(activeActor, attack.Target, suppliedTarget);

            combatController.StartCoroutine(PerformAttack());
        }
    }



    IEnumerator PerformAttack()
    {
        foreach (CombatActor actor in targetActors)
        {
            for (int i = 0; i < attack.Times; i++)
            {
                int damage = RulesSystem.instance.CalculateDamage(attack.Value, activeActor, actor);
                actor.healthEffects.TakeDamage(damage);

                for (int a = 0; a < actor.onAttackRecieved.Count; a++)
                    yield return actor.onAttackRecieved[a].Invoke(activeActor);

                yield return new WaitForSeconds(0.1f);
            }
        }


        combatController.animator.Play(nextLayerState);
    }
        



}
