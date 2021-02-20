using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCombatAnimatorDeck : CardCombatAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);
        card.transform.localPosition = new Vector3(-1000, -1000, 0);
    }

}

