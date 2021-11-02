using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterMapConfirm : EncounterMapAnimator
{

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.Init();
        gridManager.gridState = GridState.Complete;

        animator.SetBool("Confirm", false);
        animator.SetBool("IsPlaying", true);

        gridManager.hexMapController.disablePanning = false;
        gridManager.hexMapController.disableZoom = false;

    }
}
