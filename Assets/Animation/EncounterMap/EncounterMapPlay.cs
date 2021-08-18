using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterMapPlay : EncounterMapAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.Init();
        gridManager.gridState = GridState.Play;
        if (WorldSystem.instance.gridManager.CheckClearCondition())
        {
            Debug.Log("True");
            return;
        }
        else
        {
            Debug.Log("False");
        }
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
