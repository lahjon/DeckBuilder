using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllerAnimatorCardActivitiesDiscard : CombatControllerAnimatorCard
{

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);
        Debug.Log("Entered activities & discard");

        if (combat.ActiveActor == combat.Hero && combat.actorTurn == CombatActorType.Hero)
        {
            combat.NoteCardFinished(card);
            EventManager.CardFinished(card);
        }

        combat.StartCoroutine(ActivitiesDiscard());
    }

    IEnumerator ActivitiesDiscard()
    {
        foreach(CardActivitySetting a in card.activitiesOnPlay)
        {
            yield return combat.StartCoroutine(CardActivitySystem.instance.StartByCardActivity(a));
        }

        card.owner.CardResolved(card);

        if(combat.animator.GetBool("EffectsQueued"))
            yield return combat.StartCoroutine(combat.EmptyEffectQueue());

        combat.animator.SetTrigger("CardFinished");
    }
        



}
