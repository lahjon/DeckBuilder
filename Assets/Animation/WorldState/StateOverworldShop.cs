using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateOverworldShop : WorldStateAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Init(TransitionType.Normal, WorldState.OverworldShop);
        world.shopManager.EnterShop();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        world.shopManager.LeaveShop();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            WorldStateSystem.SetInOverworldShop(false);
        }
    }

}
