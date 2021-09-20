using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateTownReward : WorldStateAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Init(TransitionType.Normal, WorldState.TownReward);
        world.rewardManager.CollectRewards();
        world.characterManager.ResetDeck();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        world.SaveProgression();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

}
