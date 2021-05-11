using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterMapPlacing : EncounterMapAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.Init();
        gridManager.gridState = GridState.Placement;
        animator.SetBool("IsRotating", true);
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("IsPlacing", false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // gridManager.hexMapController.PanCamera();
    }
    
}
