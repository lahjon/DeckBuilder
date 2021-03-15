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

        if (combatController.ActiveActor == combatController.Hero)
        {
            Debug.Log("Combatcontroller Sending card to discard");
            combatController.HeroCardInProcess.animator.SetTrigger("Discarded");
        }
        else
        {
            combatController.animator.SetBool("EnemyQueued", combatController.enemiesWaiting.Count != 0);
            ((CombatActorEnemy)combatController.ActiveActor).discard.Add(card);
        }
            

        combatController.animator.SetTrigger("CardFinished");
    }
        



}
