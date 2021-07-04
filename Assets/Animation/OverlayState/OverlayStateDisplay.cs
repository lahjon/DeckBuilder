using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayStateDisplay : OverlayStateAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Init(OverlayState.Display);
        world.deckDisplayManager.deckDisplay.SetActive(true);   
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        world.deckDisplayManager.CloseDeckDisplay();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (world.deckDisplayManager.selectedCard != null)
            {
                world.deckDisplayManager.ResetCardPosition();
            }
            else
            {
                animator.SetTrigger("Clear");
            }
        }
    }

}
