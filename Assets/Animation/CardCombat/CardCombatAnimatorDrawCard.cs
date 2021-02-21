using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CardCombatAnimatorDrawCard : CardCombatAnimator
{
    public float enlargementSpeed = 0.4f;
    public float breakTolerance = 3f;

    Vector3 targetPos;
    Vector3 targetScale = new Vector3(1.0f, 1.0f, 1.0f);
    Vector3 targetAngle;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);
        card.transform.localScale = Vector3.zero;
        card.transform.localEulerAngles = Vector3.zero;
        card.MouseReact = false;
        card.selectable = true;
        card.transform.position = combatController.txtDeck.transform.position;
        card.transform.SetAsLastSibling();

        (Vector3, Vector3) TransInfo = combatController.GetPositionInHand(card);
        targetPos = TransInfo.Item1;
        targetAngle = TransInfo.Item2;
        card.selectable = true;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        card.transform.localPosition =    Vector3.Lerp(card.transform.localPosition,    targetPos,   enlargementSpeed);
        card.transform.localScale =       Vector3.Lerp(card.transform.localScale,       targetScale, enlargementSpeed);
        card.transform.localEulerAngles =    AngleLerp(card.transform.localEulerAngles, targetAngle, enlargementSpeed);

        if(Vector3.Distance(card.transform.localPosition, targetPos) < breakTolerance){
            card.transform.localPosition    = targetPos;
            card.transform.localScale       = targetScale;
            card.transform.localEulerAngles = targetAngle;
            animator.SetTrigger("DoneDrawing");
        }
    }




}

