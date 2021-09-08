using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CombatControllerAnimatorCardBlock : CombatControllerAnimatorCard
{
    List<CardEffectCarrier> blocks;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);

        blocks = card.Blocks;

        if (card.Attacks.Any())
            nextState = "Attack";
        else if (card.effectsOnPlay.Any())
            nextState = "Effects";
        else
            nextState = "Activities & Discard";


        if (blocks.Any())
            combat.StartCoroutine(ApplyBlock());
        else
            combat.animator.Play(nextLayerState);
    }

    IEnumerator ApplyBlock()
    {
        foreach(CardEffectCarrier block in blocks) { 
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
}
