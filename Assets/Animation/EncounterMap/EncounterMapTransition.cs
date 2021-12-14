using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterMapTransition : EncounterMapAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.Init();
        gridManager.gridState = GridState.Transition;
        WorldSystem.instance.scenarioMapManager.hexMapController.enableInput = false;
        WorldStateSystem.instance.transitionScreen.InOutTransitionStart();
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
