using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateBonfire : WorldStateAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Init(TransitionType.Normal, WorldState.Bonfire);
        world.bonfireManager.EnterBonfire();
        //WorldSystem.instance.uiManager.encounterUI.StartEncounter();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        world.bonfireManager.LeaveBonfire();
        //WorldSystem.instance.uiManager.encounterUI.CloseEncounter();
        //WorldStateSystem.SetInBonfire(false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
    }

}
