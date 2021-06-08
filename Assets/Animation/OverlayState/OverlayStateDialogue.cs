using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayStateDialogue : OverlayStateAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Init(OverlayState.Transition);
        Debug.Log("Enter Dialogue");
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        animator.SetBool("InDialogue", false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        if (world.dialogueManager.activeDialogue && world.dialogueManager.dialogue.sentenceDone && Input.GetMouseButtonDown(0))
        {
            world.dialogueManager.NextSentence();
        }
        else if (world.dialogueManager.activeDialogue && !world.dialogueManager.dialogue.sentenceDone && Input.GetMouseButtonDown(0))
        {
            world.dialogueManager.FinishSentence();
        }
    }

}
