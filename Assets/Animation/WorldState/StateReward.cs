using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateReward : WorldStateAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Init(TransitionType.None, WorldState.Reward);
        world.uiManager.rewardScreen.canvas.SetActive(true);
        world.uiManager.rewardScreen.OnCanvasEnable();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        world.uiManager.rewardScreen.rewardScreenCard.SetActive(false);
        world.uiManager.rewardScreen.canvas.SetActive(false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
    }

}
