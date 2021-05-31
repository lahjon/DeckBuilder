using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateReward : WorldStateAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Init(TransitionType.None, WorldState.Reward);
        if (world.rewardManager.draftAmount <= 0)
        {
            world.rewardManager.OpenRewardScreen();
        }
        else
        {
            world.rewardManager.OpenDraftMode();
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        world.combatManager.combatController.CleanUp();
        world.combatManager.combatController.content.SetActive(false);
        world.rewardManager.draftAmount = 0;
        world.rewardManager.CloseRewardScreen();
        world.cameraManager.SwapToMain();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
    }

}
