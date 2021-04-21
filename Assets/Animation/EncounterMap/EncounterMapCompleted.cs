using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterMapCompleted : EncounterMapAnimator
{

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.Init();
        gridManager.gridState = GridState.Complete;
        gridManager.bossCounter++;
        animator.SetBool("IsComplete", false);

    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        gridManager.ExitPlacement();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
