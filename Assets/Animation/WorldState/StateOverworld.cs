using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateOverworld : WorldStateAnimator
{
    HexMapController hexMapController;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Init(TransitionType.Normal, WorldState.Overworld);
        if(hexMapController == null) hexMapController = world.gridManager.hexMapController;
        world.gridManager.content.SetActive(true);
        world.gridManager.ReportEncounter();
        Debug.Log("Entering Overworld");
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Exit Overworld");
        world.gridManager.content.SetActive(false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (hexMapController.enableInput)
        {
            hexMapController.PanCamera();
            hexMapController.ZoomCamera();
            if (hexMapController.cam.transform.position != hexMapController.newPosition)
            {
                hexMapController.MoveCamera();
            }
        }
    }

}
