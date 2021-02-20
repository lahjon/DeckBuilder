using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCombatAnimatorIdle : CardCombatAnimator
{
    (Vector3 pos, Vector3 angles) TargetTransInfo;


    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("Idle OnStateEnter:" + Time.frameCount);
        SetRefs(animator);
        animator.SetBool("ReachedIdle", false);
        TargetTransInfo = combatController.GetPositionInHand(card);
    }


    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("Idle Update:" + Time.frameCount);
        card.transform.localPosition = Vector3.Lerp(card.transform.localPosition,       TargetTransInfo.pos,    20*Time.deltaTime);
        card.transform.localEulerAngles = AngleLerp(card.transform.localEulerAngles,    TargetTransInfo.angles, 20*Time.deltaTime);

        if(Vector3.Distance(card.transform.localPosition, TargetTransInfo.pos) < 2){
            card.transform.localPosition        = TargetTransInfo.pos;
            card.transform.localEulerAngles     = TargetTransInfo.angles;
            animator.SetBool("ReachedIdle",true);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("Idle OnStateExit:" + Time.frameCount);
        if(combatController.ActiveCard is null) animator.SetBool("AllowMouseOver", true);
    }
}

