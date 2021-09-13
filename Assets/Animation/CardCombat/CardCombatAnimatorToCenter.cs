using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardCombatAnimatorToCenter : CardCombatAnimator
{

    (Vector3 pos, Vector3 scale, Vector3 angles) TargetTransInfo;
    (Vector3 pos, Vector3 scale, Vector3 angles) StartTransInfo;
    AnimationCurve curve;
    private float time;
    private float speed;
    private bool reached;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("Idle OnStateEnter:" + Time.frameCount);
        SetRefs(animator);
        CombatSystem.instance.ResetSiblingIndexes();
        curve = card.transitionCurveReturn;
        StartTransInfo = TransSnapshot();
        time = 0;
        speed = 3.5f;
        reached = false;
        card.GetComponent<Image>().raycastTarget = false;
        TargetTransInfo = (CombatSystem.instance.cardHoldPos.localPosition, new Vector3(1.1f, 1.1f, 1.1f), Vector3.one);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (reached) return;

        time += speed * Time.deltaTime;        
        CardLerp(StartTransInfo, TargetTransInfo, curve.Evaluate(time));

        if (Vector3.Distance(card.transform.localPosition, TargetTransInfo.pos) < 0.5f || time > 1)
        {
            if (time < 1) CardLerp(StartTransInfo, TargetTransInfo, 1);
            reached = true;
            Debug.Log("Reached Center");
            animator.SetTrigger("ReachedCenter");
        }
    }


}

