using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldStateAnimator : StateMachineBehaviour
{
    protected static WorldSystem world;
    protected static WorldStateSystem worldStateSystem;
    WorldState setState;

    protected virtual void Init(TransitionType transactionType, WorldState aState)
    {
        Debug.Log("Enter state: " + this.GetType().ToString().Remove(0,5));

        setState = aState;

        if (world == null)
        {
            world = WorldSystem.instance;
        }
        if (worldStateSystem == null)
        {
            worldStateSystem = WorldStateSystem.instance;
        }

        Debug.Log("Do Transaction:" + transactionType);
        
        if (transactionType != TransitionType.None)
        {
            worldStateSystem.currentOverlayState = OverlayState.Transition;



            if (worldStateSystem.overrideTransitionType != TransitionType.None)
            {
                transactionType = worldStateSystem.overrideTransitionType;
            }

            switch (transactionType)
            {
                case TransitionType.Normal:
                    worldStateSystem.transitionScreen.NormalTransitionStart();
                    break;

                case TransitionType.EnterAct:
                    worldStateSystem.transitionScreen.EnterActTransitionStart();
                    break;
                
                default:
                    break;
            }
        }

        worldStateSystem.currentWorldState = setState;

        
        worldStateSystem.overrideTransitionType = TransitionType.None;
    }

    // protected void TransitionCallback()
    // {
    //     worldStateSystem.currentWorldState = setState;
    // }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}
