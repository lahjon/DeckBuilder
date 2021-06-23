﻿using System.Collections;
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

        card.animator.SetBool("HasTarget", false);
        card.GetComponent<BezierFollow>().StartAnimation(card.animator.GetBool("ToCardPileDiscard"));
        card.animator.SetBool("ToCardPileDiscard", true);
    }

}

