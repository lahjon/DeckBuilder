using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterMapPlacing : EncounterMapAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.Init();
        
        gridManager.canvas.gameObject.SetActive(true);
        gridManager.gridState = GridState.Placing;
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        gridManager.canvas.gameObject.SetActive(false);
        animator.SetBool("IsPlacing", false);
    }

}
