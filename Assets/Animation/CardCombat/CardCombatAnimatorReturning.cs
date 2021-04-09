using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCombatAnimatorReturning : CardCombatAnimator
{
    (Vector3 pos, Vector3 scale, Vector3 angles) TargetTransInfo;
    (Vector3 pos, Vector3 scale, Vector3 angles) StartTransInfo;
    AnimationCurve curve;
    private float time;
    private float speed;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("Idle OnStateEnter:" + Time.frameCount);
        SetRefs(animator);
        combatController.ResetSiblingIndexes();
        curve = card.transitionCurveReturn;
        StartTransInfo = TransSnapshot();
        time = 0;
        speed = 3.5f;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        time += speed * Time.deltaTime;

        //Moving this here as card drawing makes this vulnerable to lock down;
        (Vector3 pos, Vector3 angles) tempTransInfo = combatController.GetPositionInHand(card);
        TargetTransInfo = (tempTransInfo.pos, Vector3.one, tempTransInfo.angles);
        CardLerp(StartTransInfo, TargetTransInfo, curve.Evaluate(time));

        if (Vector3.Distance(card.transform.localPosition, TargetTransInfo.pos) < 0.5f || time > 1)
        {
            if (time < 1) CardLerp(StartTransInfo, TargetTransInfo, curve.Evaluate(time));
            animator.SetTrigger("ReturnedToIdle");
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (combatController.ActiveCard is null) animator.SetBool("AllowMouseOver", true);
    }
}