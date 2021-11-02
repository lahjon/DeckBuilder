using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterMapRotating : EncounterMapAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.Init();
        
        gridManager.canvas.gameObject.SetActive(true);
        gridManager.gridState = GridState.Rotating;
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        gridManager.canvas.gameObject.SetActive(false);
        animator.SetBool("IsRotating", false);
    }

}
