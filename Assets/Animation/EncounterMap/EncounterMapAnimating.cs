using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EncounterMapAnimating : EncounterMapAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.Init();
        gridManager.gridState = GridState.Animating;
        gridManager.hexMapController.disablePanning = true;
        gridManager.hexMapController.disableZoom = true;
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        gridManager.hexMapController.disablePanning = false;
        gridManager.hexMapController.disableZoom = false;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
