using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CardCombatAnimatorSelected : CardCombatAnimator
{
    (Vector3 pos, Vector3 scale, Vector3 angles) TargetTransInfo;
    (Vector3 pos, Vector3 scale, Vector3 angles) StartTransInfo;


    float duration = 0.2f;
    float time;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);
        StartTransInfo = (card.transform.localPosition, card.transform.localScale, card.transform.localEulerAngles);
        TargetTransInfo = (new Vector3(CombatSystem.instance.GetPositionInHand(card).Position.x, 200, 0), 1.1f * Vector3.one, Vector3.zero);
        time = 0;

        WorldSystem.instance.toolTipManager.canShow = false;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        time += Time.deltaTime;

        if (time < duration)
            CardLerp(StartTransInfo, TargetTransInfo, card.transitionCurveReturn.Evaluate(time / duration)); //fucking time.Deltatime??? messed up.
        else
            CardLerp(StartTransInfo, TargetTransInfo, 1);

        card.cardCollider.MirrorCardTrans();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        card.selected = false;
        WorldSystem.instance.toolTipManager.canShow = true;
        card.cardCollider.gameObject.SetActive(false);
    }
}
