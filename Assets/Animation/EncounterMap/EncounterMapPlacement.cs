using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterMapPlacement : EncounterMapAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.Init();
        
        gridManager.gridState = GridState.Placement;
        gridManager.hexMapController.disableInput = true;
        foreach (HexTile tile in gridManager.highlightedTiles)
        {
            if (tile.tileState == TileState.InactiveHighlight)
            {
                tile.tileState = TileState.Inactive;
            }
        }
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
    
}