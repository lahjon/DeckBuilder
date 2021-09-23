using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CardCombatAnimatorSelectedTarget : CardCombatAnimator
{
    (Vector3 pos, Vector3 scale, Vector3 angles) TargetTransInfo;
    (Vector3 pos, Vector3 scale, Vector3 angles) StartTransInfo;


    float duration = 0.2f;
    float time;
    SelectionPath selectionPath;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);
        card.transform.SetAsLastSibling();

        StartTransInfo = (card.transform.localPosition, card.transform.localScale, card.transform.localEulerAngles);
        TargetTransInfo = (new Vector3(0, 150, 0), Vector3.one, Vector3.zero);
        time = 0;

        WorldSystem.instance.toolTipManager.canShow = false;

        //selectionPath = CombatSystem.instance.selectionPath;
        //DOTween.To(() => 0, x => { }, 0, timeDelay).OnComplete( () => selectionPath.StartFollow());
        //selectionPath.StartFollow(card);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        time += Time.deltaTime;

        if (time < duration)
            CardLerp(StartTransInfo, TargetTransInfo, time/duration, card.transitionCurveReturn); //fucking time.Deltatime??? messed up.
        else
            CardLerp(StartTransInfo, TargetTransInfo, 1);

        //selectionPath.FollowPath();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        card.selected = false;
        WorldSystem.instance.toolTipManager.canShow = true;
        //selectionPath.StopFollow(card);
    }
}
