using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CardCombatAnimatorMouseOver : CardCombatAnimator
{
    public float enlargementSpeed = 4f;

    (Vector3 pos, Vector3 scale, Vector3 angles) TargetTransInfo;
    (Vector3 pos, Vector3 scale, Vector3 angles) StartTransInfo;
    AnimationCurve curve;

    float time;


    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);
        curve = card.transitionCurveReturn;
        card.transform.SetAsLastSibling();
        StartTransInfo = (card.transform.localPosition, card.transform.localScale, card.transform.localEulerAngles);

        (Vector3 pos, Vector3 angles) tempTransInfo = CombatSystem.instance.GetPositionInHand(card);
        TargetTransInfo = (tempTransInfo.pos, Vector3.one, tempTransInfo.angles);
        card.cardCollider.SetTransform(TargetTransInfo);
        card.cardCollider.gameObject.SetActive(true);
        time = 0;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        time += enlargementSpeed*Time.deltaTime;

        // ugly but dont know what else todo
        card.transform.SetAsLastSibling();
        TargetTransInfo = (new Vector3(CombatSystem.instance.GetPositionInHand(card).Position.x, 200, 0), 1.1f*Vector3.one, Vector3.zero);
        if (time < 1)
            CardLerp(StartTransInfo, TargetTransInfo, curve.Evaluate(time)); //fucking time.Deltatime??? messed up.
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("MouseOver StateExit: Sending Refresh idle trigger" + Time.frameCount);
        



        CombatSystem.instance.ResetSiblingIndexes();
    }

}

