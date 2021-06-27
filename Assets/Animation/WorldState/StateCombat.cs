using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateCombat : WorldStateAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Init(TransitionType.Normal, WorldState.Combat);

        world.toolTipManager.canvas.worldCamera = world.cameraManager.combatCamera;

        world.cameraManager.SwapToCombat();
        CombatSystem.instance.StartCombat();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        WorldStateSystem.SetInCombat(false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
    }

}
