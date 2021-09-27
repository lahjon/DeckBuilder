using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayStateDisplay : OverlayStateAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Init(OverlayState.Display);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (world.deckDisplayManager.selectedCard != null)
            {
                world.deckDisplayManager.DeactivateDisplayCard();
            }
            else
            {
                animator.SetTrigger("Clear");
            }
        }
    }

}
