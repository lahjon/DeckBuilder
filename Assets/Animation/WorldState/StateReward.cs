using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateReward : WorldStateAnimator
{
    int[] keys = new int[] { 1,2,3,4,5};
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Init(TransitionType.None, WorldState.Reward);
        if (world.rewardManager.draftAmount <= 0)
        {
            world.rewardManager.OpenCombatRewardScreen();
        }
        else
        {
            world.rewardManager.OpenDraftMode();
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CombatSystem.instance.CleanUp(); //kanske lägga detta i "Waiting" state för combatSystem?
        CombatSystem.instance.content.SetActive(false); //kanske lägga detta i "Waiting" state för combatSystem?
        world.rewardManager.draftAmount = 0;
        world.rewardManager.rewardScreenCombat.RemoveRewardScreen();
        world.cameraManager.SwapToMain();
        CombatSystem.instance.animator.SetTrigger("Reset");
        CombatSystem.instance.animator.SetBool("HasWon",false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        for(int i = 0; i < keys.Length && i < WorldSystem.instance.rewardManager.rewardScreenCombat.content.transform.childCount; i++)
        {
            if (Input.GetKeyDown(keys[i].ToString()) && WorldStateSystem.instance.currentWorldState == WorldState.Reward)
            {
                WorldSystem.instance.rewardManager.rewardScreenCombat.content.transform.GetChild(keys[i] - 1).GetComponent<Reward>().OnClick();
                break;
            }
        }
        if(Input.GetKeyDown(KeyCode.Space ) && WorldStateSystem.instance.currentWorldState == WorldState.Reward)
            WorldSystem.instance.rewardManager.rewardScreenCombat.RemoveRewardScreen();
    }

}
