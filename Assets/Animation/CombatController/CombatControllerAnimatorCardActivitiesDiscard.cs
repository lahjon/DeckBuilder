using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllerAnimatorCardActivitiesDiscard : CombatControllerAnimatorCard
{
    List<CombatActor> targetActors = new List<CombatActor>();

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);
        Debug.Log("Entered activities & discard");
        combatController.StartCoroutine(ActivitiesDiscard());
    }

    IEnumerator ActivitiesDiscard()
    {
        foreach(CardActivitySetting a in card.activities)
        {
            yield return combatController.StartCoroutine(CardActivitySystem.instance.StartByCardActivity(a));
        }

        Debug.Log("Combatcontroller Sending card to discard");
        combatController.animator.SetTrigger("CardFinished");
        combatController.HeroCardInProcess.animator.SetTrigger("Discarded");
    }
        



}
