using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class OverlayStateRewardScreen : OverlayStateAnimator
{
    //float timeDelay = 0.5f;
    //bool canExit;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Init(OverlayState.RewardScreen);
        world.rewardManager.rewardScreen.OpenScreen();
        //Helpers.DelayForSeconds(timeDelay, () => canExit = true);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        world.rewardManager.rewardScreen.ClearScreen();
        base.OnStateExit(animator, stateInfo, layerIndex);
        
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // if (canExit && Input.GetMouseButtonDown(0))
        //     WorldStateSystem.TriggerClear();
        
    }

}
