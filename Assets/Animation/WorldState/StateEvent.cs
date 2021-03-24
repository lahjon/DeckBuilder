using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateEvent : WorldStateAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Init(TransitionType.Normal, WorldState.Event);
        WorldSystem.instance.uiManager.encounterUI.StartEncounter();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        WorldSystem.instance.uiManager.encounterUI.CloseEncounter();
        WorldStateSystem.SetInEvent(false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
    }

}
