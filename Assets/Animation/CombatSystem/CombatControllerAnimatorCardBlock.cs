using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllerAnimatorCardBlock : CombatControllerAnimatorCard
{
    CardEffectInfo block;

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
            combat.animator.Play(nextLayerState);
        else
            combat.StartCoroutine(ApplyBlock());
    }

    IEnumerator ApplyBlock()
    {
        List<CombatActor> targets = combat.GetTargets(combat.ActiveActor, block.Target, suppliedTarget);
        for (int i = 0; i < block.Times; i++)
        {
            foreach (CombatActor actor in targets)
                yield return combat.StartCoroutine(actor.ChangeBlock(block.Value));

            if(block.Target == CardTargetType.EnemyRandom && i != block.Times -1) // redraw if random, though doubt it ever will be
                targets = combat.GetTargets(combat.ActiveActor, block.Target, suppliedTarget);
        }
        combat.animator.Play(nextLayerState);
    }
        



}
