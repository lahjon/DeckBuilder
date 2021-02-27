using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCombatAnimatorIdle : CardCombatAnimator
{
    private static float speed = 40f;
    private static float limit = 0.5f;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("Idle OnStateEnter:" + Time.frameCount);
        SetRefs(animator);        
        card.fanDegreeCurrent = combatController.GetCurrentDegree(card);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Moving this here as card drawing makes this vulnerable to lock down;
        card.fanDegreeTarget = combatController.GetTargetDegree(card);
        
        if(Mathf.Abs(card.fanDegreeTarget-card.fanDegreeCurrent) > limit)
        {
            if(Mathf.Sign(card.fanDegreeTarget - card.fanDegreeCurrent) == 1)
                card.fanDegreeCurrent = Mathf.Clamp(card.fanDegreeCurrent + speed*Time.deltaTime,-200, card.fanDegreeTarget);
            else
                card.fanDegreeCurrent = Mathf.Clamp(card.fanDegreeCurrent - speed*Time.deltaTime, card.fanDegreeTarget, 200);

            combatController.SetCardTransFromDegree(card, card.fanDegreeCurrent);
        }
        else {
            Debug.Log("Close enough for idle to end: " + card.fanDegreeCurrent + " , " + card.fanDegreeTarget);
            card.fanDegreeCurrent = card.fanDegreeTarget;
            combatController.SetCardTransFromDegree(card, card.fanDegreeTarget);
            animator.SetBool("NeedFan", false);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(combatController.ActiveCard is null) animator.SetBool("AllowMouseOver", true);
    }
}

