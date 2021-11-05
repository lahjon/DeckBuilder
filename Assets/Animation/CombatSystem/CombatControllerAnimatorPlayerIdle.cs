using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllerAnimatorPlayerIdle : CombatControllerAnimator
{
    CombatActor hero;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);
        combat.combatStateType = CombatStateType.Idle;
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        combat.combatStateType = CombatStateType.NotIdle;
    }
}
