using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCombatAnimatorSelectedTarget : CardCombatAnimator
{
    Vector3 targetPos;
    Vector3 targetScale = new Vector3(1.15f, 1.15f, 1.15f);
    Vector3 targetAngle = Vector3.zero;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);
        card.transform.SetAsLastSibling();
        targetPos = new Vector3(combatController.GetPositionInHand(card).Position.x, 200, 0);

        card.transform.localPosition = targetPos;
        card.transform.localScale = targetScale;
        card.transform.localEulerAngles = targetAngle;
    }

}
