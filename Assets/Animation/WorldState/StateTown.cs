using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StateTown : WorldStateAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Init(TransitionType.Town, WorldState.Town);
        WorldSystem.instance.townManager.EnterTown();
        if (world.rewardManager.uncollectedReward?.Any() == true) WorldStateSystem.SetInTownReward(true);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        world.townManager.ExitTown();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
    }
}
