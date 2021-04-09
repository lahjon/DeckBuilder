using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllerAnimatorCardEffects : CombatControllerAnimatorCard
{
    (CardEffect effect, List<CombatActor> targets) effectAndTarget;

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
            effectAndTarget = combatController.GetTargets(combatController.ActiveActor, e, suppliedTarget);

            for (int i = 0; i < effectAndTarget.effect.Times; i++)
                foreach (CombatActor actor in effectAndTarget.targets)
                    actor.RecieveEffectNonDamageNonBlock(effectAndTarget.effect);
        }

        combatController.animator.SetTrigger("CardEffectsHandled");
        yield return null;
    }
        



}
