using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateCombatReward : WorldStateAnimator
{
    int[] keys = new int[] { 1,2,3,4,5};
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Init(TransitionType.None, WorldState.CombatReward);
        Debug.Log("Entering Reward");
        CombatSystem.instance.CleanUpEnemies();
        world.rewardManager.OpenCombatRewardScreen();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CombatSystem.instance.CleanUpScene();
        CombatSystem.instance.content.SetActive(false);
        world.rewardManager.rewardScreenCombat.RemoveRewardScreen();
        world.cameraManager.SwapToMain();
        world.toolTipManager.DisableTips();
        CombatSystem.instance.animator.SetTrigger("Reset");
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        for(int i = 0; i < keys.Length && i < WorldSystem.instance.rewardManager.rewardScreenCombat.content.transform.childCount; i++)
        {
            if (Input.GetKeyDown(keys[i].ToString()) && WorldStateSystem.instance.currentWorldState == WorldState.CombatReward)
            {
                WorldSystem.instance.rewardManager.rewardScreenCombat.content.transform.GetChild(keys[i] - 1).GetComponent<Reward>().OnClick();
                break;
            }
        }
        if(Input.GetKeyDown(KeyCode.Space ) && WorldStateSystem.instance.currentWorldState == WorldState.CombatReward)
            WorldSystem.instance.rewardManager.rewardScreenCombat.RemoveRewardScreen();
    }

}
