using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCombatAnimatorForeshadowed : CardCombatAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("Idle OnStateEnter:" + Time.frameCount);
        SetRefs(animator);        
        card.fanDegreeCurrent = CombatSystem.instance.GetCurrentDegree(card);
    }
}

