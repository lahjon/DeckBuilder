using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateEventReward : WorldStateAnimator
{
    int[] keys = new int[] { 1,2,3,4,5};
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Init(TransitionType.Normal, WorldState.EventReward);
        world.combatRewardManager.CollectRewards();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

}
