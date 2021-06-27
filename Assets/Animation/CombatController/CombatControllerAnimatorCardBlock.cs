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
            CombatSystem.instance.animator.Play(nextLayerState);
        else
            CombatSystem.instance.StartCoroutine(ApplyBlock());
    }

    IEnumerator ApplyBlock()
    {
        List<CombatActor> targets = CombatSystem.instance.GetTargets(CombatSystem.instance.ActiveActor, block, suppliedTarget);
        for (int i = 0; i < block.Times; i++)
        {
            foreach (CombatActor actor in targets)
                yield return CombatSystem.instance.StartCoroutine(actor.ChangeBlock(block.Value));

            if(block.Target == CardTargetType.EnemyRandom && i != block.Times -1) // redraw if random, though doubt it ever will be
                targets = CombatSystem.instance.GetTargets(CombatSystem.instance.ActiveActor, block, suppliedTarget);
        }

        CombatSystem.instance.animator.Play(nextLayerState);
    }
        



}
