using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllerAnimatorCardBlock : CombatControllerAnimatorCard
{
    CardEffect block;
    List<CombatActor> targetActors = new List<CombatActor>();

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
            targetActors.Clear();
            targetActors = combatController.GetTargets(combatController.ActiveActor, block.Target, suppliedTarget);

            combatController.StartCoroutine(GetBlock());
        }
    }

    IEnumerator GetBlock()
    {
        Debug.Log("Starting block recieving");
        foreach (CombatActor actor in targetActors)
        {
            for (int i = 0; i < block.Times; i++)
            {
                actor.healthEffects.RecieveBlock(block.Value);
                yield return new WaitForSeconds(0.3f);
            }
        }

        combatController.animator.Play(nextLayerState);
    }
        



}
