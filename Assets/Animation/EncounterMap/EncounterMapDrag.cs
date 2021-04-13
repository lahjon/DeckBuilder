using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterMapDrag : EncounterMapAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Init();
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("IsDraging", false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (gridManager.hoverTilePosition != Vector3.zero)
        {
            tile.transform.position = gridManager.hoverTilePosition;
        }
        else
        {
            tile.transform.position = WorldSystem.instance.cameraManager.mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, gridManager.transform.position.z));
        }

        if (Input.GetMouseButtonDown(0))
        {
            tile.CheckPlacement();
        }
    }
}
