using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayStateDisplay : OverlayStateAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Init(OverlayState.Display);
        Debug.Log("IN DISPLAY");
        world.deckDisplayManager.deckDisplay.SetActive(true);
        world.deckDisplayManager.UpdateAllCards();    
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        world.deckDisplayManager.CloseDeckDisplay();
        animator.SetBool("InDisplay", false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (world.deckDisplayManager.selectedCard != null)
            {
                world.deckDisplayManager.selectedCard.ResetCardPosition();
            }
            else
            {
                animator.SetTrigger("Clear");
            }
        }
    }

}
