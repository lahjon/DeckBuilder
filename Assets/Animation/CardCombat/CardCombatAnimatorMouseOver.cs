﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CardCombatAnimatorMouseOver : CardCombatAnimator
{
    public float enlargementSpeed = 0.7f;

    Vector3 targetPos;
    Vector3 targetScale = new Vector3(1.1f, 1.1f, 1.1f);
    Vector3 targetAngle = Vector3.zero;

    float TimeUntilToolTip = 1f;

    float time;
    bool toolTipOn;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);
        card.transform.SetAsLastSibling();
        targetPos = new Vector3(combatController.GetPositionInHand(card).Position.x, 200, 0);
        time = 0;
        toolTipOn = false;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        time += Time.deltaTime;
        if(!toolTipOn && time > TimeUntilToolTip)
        {
            toolTipOn = true;
            card.tooltipController.ShowHide(true);
        }

        card.transform.SetAsLastSibling();
        card.transform.localPosition = Vector3.Lerp(card.transform.localPosition, targetPos, enlargementSpeed);
        card.transform.localScale = Vector3.Lerp(card.transform.localScale, targetScale, enlargementSpeed);
        card.transform.localEulerAngles = AngleLerp(card.transform.localEulerAngles, targetAngle, enlargementSpeed);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("MouseOver StateExit: Sending Refresh idle trigger" + Time.frameCount);
        combatController.ResetSiblingIndexes();
        animator.SetBool("ReachedIdle",false);
        card.tooltipController.ShowHide(false);
    }

}

