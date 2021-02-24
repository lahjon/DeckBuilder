using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CardCombatAnimatorMouseOver : CardCombatAnimator
{
    public float enlargementSpeed = 40f;

    (Vector3 pos, Vector3 scale, Vector3 angles) TargetTransInfo;
    (Vector3 pos, Vector3 scale, Vector3 angles) StartTransInfo;

    float TimeUntilToolTip = 1f;

    float time;
    bool toolTipOn;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);
        card.transform.SetAsLastSibling();
        StartTransInfo = (card.transform.localPosition, card.transform.localScale, card.transform.localEulerAngles);

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

        // ugly but dont know what else todo
        card.transform.SetAsLastSibling();

        TargetTransInfo = (new Vector3(combatController.GetPositionInHand(card).Position.x, 200, 0), new Vector3(1.1f, 1.1f, 1.1f), new Vector3(0, 0, 0));
        if (time* enlargementSpeed < 1)
            CardLerp(StartTransInfo, TargetTransInfo, 1f); //fucking time.Deltatime??? messed up.
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("MouseOver StateExit: Sending Refresh idle trigger" + Time.frameCount);
        combatController.ResetSiblingIndexes();
        animator.SetBool("ReachedIdle",false);
        card.tooltipController.ShowHide(false);
    }

}

