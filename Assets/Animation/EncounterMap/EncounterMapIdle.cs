using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EncounterMapIdle : EncounterMapAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.Init();
        gridManager.gridState = GridState.Placement;
        gridManager.HighlightEntries();
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
 
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // if (Input.GetMouseButtonDown(0))
        // {
        //     animator.SetBool("IsPanning", true);
        // }
    }
}
