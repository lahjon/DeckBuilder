using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllerAnimatorCardEffects : CombatControllerAnimatorCard
{
    (CardEffectInfo effect, List<CombatActor> targets) effectAndTarget;

    CardTargetType? lastTargetType;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);
        lastTargetType = null;
        CombatSystem.instance.StartCoroutine(DealEffects());
    }

    IEnumerator DealEffects()
    {
        //Debug.Log("Starting effets transmittion");

        foreach(CardEffectInfo e in card.effectsOnPlay)
        {
            effectAndTarget = CombatSystem.instance.GetTargets(CombatSystem.instance.ActiveActor, e, suppliedTarget);

            for (int i = 0; i < effectAndTarget.effect.Times; i++)
                foreach (CombatActor actor in effectAndTarget.targets)
                    actor.RecieveEffectNonDamageNonBlock(effectAndTarget.effect);
        }

        CombatSystem.instance.animator.SetTrigger("CardEffectsHandled");
        yield return null;
    }
        



}
