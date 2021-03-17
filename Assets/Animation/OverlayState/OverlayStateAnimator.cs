using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayStateAnimator : StateMachineBehaviour
{
    protected static WorldSystem world;
    protected static WorldStateSystem worldStateSystem;

    protected virtual void Init(OverlayState overlayState)
    {
        Debug.Log("Enter overlay state: " + this.GetType().ToString().Remove(0,5));

        if (world == null)
        {
            world = WorldSystem.instance;
        }
        if (worldStateSystem == null)
        {
            worldStateSystem = WorldStateSystem.instance;
        }

        worldStateSystem.currentOverlayState = overlayState;
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        worldStateSystem.currentOverlayState = OverlayState.None;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}
