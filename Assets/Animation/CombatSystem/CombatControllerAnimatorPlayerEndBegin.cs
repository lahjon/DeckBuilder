using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllerAnimatorPlayerEndBegin : CombatControllerAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);
        combat.forcePlayCards = true;
        combat.acceptEndTurn = false;
        List<CardCombat> handCopy = new List<CardCombat>(combat.Hand);
        foreach(CardCombat card in handCopy)
        {
            if (card.HasProperty(CardSingleFieldPropertyType.Inescapable))
            {
                combat.CardQueue.Enqueue((card, CombatSystem.instance.TargetedEnemy));
                combat.HeroCardsWaiting.Enqueue(card);
                combat.Hand.Remove(card);
            }
        }

        Debug.Log("**********************************" + combat.CardQueue.Count);

        combat.animator.SetBool("CardsQueued", combat.CardQueue.Count != 0);
        combat.animator.SetTrigger("PlayerTurnEnded");

    }





}
