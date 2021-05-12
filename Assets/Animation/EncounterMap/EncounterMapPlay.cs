using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterMapPlay : EncounterMapAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.Init();
        gridManager.gridState = GridState.Play;
        foreach (HexTile tile in gridManager.highlightedTiles)
        {
            Debug.Log(tile.coord);
            tile.StopFadeInOutColor();
        }
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
