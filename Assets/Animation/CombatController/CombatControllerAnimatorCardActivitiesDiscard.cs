using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllerAnimatorCardActivitiesDiscard : CombatControllerAnimatorCard
{

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);
        //Debug.Log("Entered activities & discard");
        combatController.StartCoroutine(ActivitiesDiscard());
    }

    IEnumerator ActivitiesDiscard()
    {
        foreach(CardActivitySetting a in card.activitiesOnPlay)
        {
            yield return combatController.StartCoroutine(CardActivitySystem.instance.StartByCardActivity(a));
        }

        card.owner.CardResolved(card);
           
        combatController.animator.SetTrigger("CardFinished");
    }
        



}
