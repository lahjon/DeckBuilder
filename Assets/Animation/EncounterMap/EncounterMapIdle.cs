using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EncounterMapIdle : EncounterMapAnimator
{
    bool _highlightedEntries;

    public bool highlightedEntries
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
        gridManager.gridState = GridState.Placement;

        //Debug.Log(highlightedEntries);
        if (!highlightedEntries)
        {
            //Debug.Log("Start Highlight!");
            gridManager.HighlightEntries();
            highlightedEntries = true;
        }

    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!animator.GetBool("IsPanning") || animator.GetBool("IsPlacing"))
        {
            highlightedEntries = false;
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
