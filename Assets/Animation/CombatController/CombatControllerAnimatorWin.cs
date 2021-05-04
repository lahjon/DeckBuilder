using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CombatControllerAnimatorWin : CombatControllerAnimator
{
    CombatActor hero;
    float timeDelay = 1.0f;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);
        DOTween.To(() => 0, x => { }, 0, timeDelay).OnComplete( () => WorldStateSystem.SetInReward(true) );
        combatController.combatOverlay.AnimateVictorious();
        combatController.EndTurn();
        
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }


}
