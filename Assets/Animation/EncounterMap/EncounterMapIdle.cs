using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EncounterMapIdle : EncounterMapAnimator
{
    bool highlightedEntries;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.Init();
        gridManager.gridState = GridState.Placement;

        if (!highlightedEntries)
        {
            gridManager.HighlightEntries(); 
            highlightedEntries = true;
        }
        
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!animator.GetBool("IsPanning"))
        {
            highlightedEntries = false;
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // if (Input.GetMouseButtonDown(0))
        // {
        //     animator.SetBool("IsPanning", true);
        // }
    }
}
