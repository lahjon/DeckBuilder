using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayStateEscapeMenu : OverlayStateAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Init(OverlayState.EscapeMenu);
        world.uiManager.escapeMenu.Activate();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        world.uiManager.escapeMenu.Deactivate();
        animator.SetBool("InEscapeMenu", false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (world.uiManager.escapeMenu.abandonWindow.activeSelf)
            {
                world.uiManager.escapeMenu.abandonWindow.SetActive(false);
            }
            else if(world.uiManager.escapeMenu.mainMenuWindow.activeSelf)
            {
                world.uiManager.escapeMenu.mainMenuWindow.SetActive(false);
            }
            else
            {
                animator.SetTrigger("Clear");
            }
        }
    }

}
