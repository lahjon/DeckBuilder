using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class CombatControllerAnimatorQueueResolver : CombatControllerAnimator
{
    (CardCombat card, CombatActor suppliedTarget) cardWaiting;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);
        
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (combat.acceptProcess) CheckCardProcess(animator);
    }

    public void CheckCardProcess(Animator animator)
    {
        cardWaiting = combat.CardQueue.Dequeue();
        Debug.Log("Starting processing of card" + cardWaiting.card.cardName);
        animator.SetBool("CardsQueued", combat.CardQueue.Count != 0);
        if (cardWaiting.card.targetRequired && !combat.EnemiesInScene.Contains((CombatActorEnemy)cardWaiting.suppliedTarget))
        {
            combat.Hand.Add(cardWaiting.card);
            cardWaiting.card.selectable = true;
            combat.RefreshHandPositions();
            combat.cEnergy += cardWaiting.card.displayCost;
            cardWaiting.card.animator.SetTrigger("Unplayable");
        }
        else
        {
            combat.InProcessCard = cardWaiting.card;
            combat.InProcessTarget = cardWaiting.suppliedTarget;
            cardWaiting.card.animator.SetTrigger("CanPlay");
            animator.SetTrigger("CardCanProcess");
        }
    }

}
