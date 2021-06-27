using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllerAnimatorCardBlock : CombatControllerAnimatorCard
{
    CardEffectInfo block;
    (CardEffectInfo effect, List<CombatActor> targets) effectAndTarget;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);
        block = card.Block;

        if (card.Damage.Value != 0)
            nextState = "Attack";
        else if (card.effectsOnPlay.Count != 0)
            nextState = "Effects";
        else
            nextState = "Activities & Discard";


        if (block.Value == 0)
            CombatSystem.instance.animator.Play(nextLayerState);
        else
        {
            effectAndTarget = CombatSystem.instance.GetTargets(CombatSystem.instance.ActiveActor, block, suppliedTarget);
            CombatSystem.instance.StartCoroutine(GetBlock());
        }
    }

    IEnumerator GetBlock()
    {
        for (int i = 0; i < effectAndTarget.effect.Times; i++)
        {
            foreach (CombatActor actor in effectAndTarget.targets)
            {
                yield return CombatSystem.instance.StartCoroutine(actor.ChangeBlock(effectAndTarget.effect.Value));
            }
        }

        CombatSystem.instance.animator.Play(nextLayerState);
    }
        



}
