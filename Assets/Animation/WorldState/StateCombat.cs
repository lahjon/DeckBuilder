using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateCombat : WorldStateAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Init(TransitionType.Normal, WorldState.Combat);
        //world.cameraManager.CameraGoto(WorldState.Combat, true);
        world.combatManager.combatController.StartCombat();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        world.combatManager.combatController.content.gameObject.SetActive(false);
        WorldStateSystem.SetInCombat(false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
    }

}
