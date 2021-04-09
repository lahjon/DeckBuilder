using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllerAnimatorCardBlock : CombatControllerAnimatorCard
{
    CardEffect block;
    (CardEffect effect, List<CombatActor> targets) effectAndTarget;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);
        block = card.Block;

        if (card.Damage.Value != 0)
            nextState = "Attack";
        else if (card.Effects.Count != 0)
            nextState = "Effects";
        else
            nextState = "Activities & Discard";


        if (block.Value == 0)
            combatController.animator.Play(nextLayerState);
        else
        {
            effectAndTarget = combatController.GetTargets(combatController.ActiveActor, block, suppliedTarget);
            combatController.StartCoroutine(GetBlock());
        }
    }

    IEnumerator GetBlock()
    {
        for (int i = 0; i < effectAndTarget.effect.Times; i++)
        {
            foreach (CombatActor actor in effectAndTarget.targets)
            {
                yield return combatController.StartCoroutine(actor.ChangeBlock(effectAndTarget.effect.Value));
            }
        }

        combatController.animator.Play(nextLayerState);
    }
        



}
