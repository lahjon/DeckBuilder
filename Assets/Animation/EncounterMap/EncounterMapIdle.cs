using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EncounterMapIdle : EncounterMapAnimator
{
    bool _highlightedEntries;

    public bool highlightedChoosable
    {
        get => _highlightedEntries;
        set
        {
            _highlightedEntries = value;
        }
    }
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.Init();
        gridManager.gridState = GridState.Placing;

        if (!highlightedChoosable)
        {
            gridManager.HighlightChoosable();
            highlightedChoosable = true;
        }

    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!animator.GetBool("IsPanning") || animator.GetBool("IsPlacing"))
        {
            highlightedChoosable = false;
            //Debug.Log("REMOVING!");
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
