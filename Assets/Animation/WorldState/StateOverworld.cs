using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateOverworld : WorldStateAnimator
{
    HexMapController hexMapController;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Init(TransitionType.Normal, WorldState.Overworld);
        world.gridManager.content.SetActive(true);
        hexMapController = world.gridManager.hexMapController;
        if (world.subAct > 1) world.gridManager.ExpandMap();
        //add expand if subact
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        world.gridManager.content.SetActive(false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!hexMapController.disablePanning)
        {
            hexMapController.PanCamera();
        }
        if(!hexMapController.disableZoom)
        {
            hexMapController.ZoomCamera();
        }
        if (hexMapController.cam.transform.position != hexMapController.newPosition)
        {
            hexMapController.MoveCamera();
        }
    }

}
