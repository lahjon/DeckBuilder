using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllerAnimatorCardEffects : CombatControllerAnimatorCard
{
    List<CombatActor> targetActors = new List<CombatActor>();

    CardTargetType? lastTargetType;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);
        lastTargetType = null;
        combatController.StartCoroutine(DealEffects());
    }

    IEnumerator DealEffects()
    {
        Debug.Log("Starting effets transmittion");

        foreach(CardEffect e in card.Effects)
        {
            if (lastTargetType == null || lastTargetType != e.Target)
            {
                targetActors.Clear();
                targetActors = combatController.GetTargets(combatController.ActiveActor, e.Target, suppliedTarget);
            }

            foreach (CombatActor actor in targetActors)
            {
                actor.healthEffects.RecieveEffectNonDamageNonBlock(e);
                yield return new WaitForSeconds(0.1f);
            }
        }

        combatController.animator.SetTrigger("CardEffectsHandled");
    }
        



}
