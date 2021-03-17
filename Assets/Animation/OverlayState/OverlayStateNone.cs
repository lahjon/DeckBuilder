using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayStateNone : OverlayStateAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Init(OverlayState.None);
        
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            WorldStateSystem.SetInEscapeMenu(true);
        }
    }
}
