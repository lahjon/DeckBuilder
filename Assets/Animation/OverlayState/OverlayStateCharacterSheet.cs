using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayStateCharacterSheet : OverlayStateAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Init(OverlayState.CharacterSheet);
        world.characterManager.characterSheet.OpenCharacterSheet();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        world.characterManager.characterSheet.CloseCharacterSheet();
        animator.SetBool("InCharacterSheet", false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.C))
        {
            animator.SetTrigger("Clear");
        }
    }
}
