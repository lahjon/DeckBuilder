using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardCombatAnimatorDiscarding : CardCombatAnimator
{
    public ParticleSystem animationSystem;
    GameObject animationObject;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);

        CombatSystem.instance.DiscardCard(card);
        card.animator.SetBool("HasTarget", false);
        card.GetComponent<BezierFollow>().StartAnimation(1);
        card.animator.SetBool("ToCardPileDiscard", true);
    }

}

