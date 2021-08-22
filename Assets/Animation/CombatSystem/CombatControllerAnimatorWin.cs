using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CombatControllerAnimatorWin : CombatControllerAnimator
{
    float timeDelay = 1.0f;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);
        Debug.Log("Entered Won State!");
        DOTween.To(() => 0, x => { }, 0, timeDelay).OnComplete( () => WorldStateSystem.SetInReward(true) );
        combat.combatOverlay.AnimateVictorious();
        combat.EndTurn();
    }

}
