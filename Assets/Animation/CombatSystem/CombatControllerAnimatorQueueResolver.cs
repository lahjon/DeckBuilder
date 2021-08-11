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
        cardWaiting = CombatSystem.instance.CardQueue.Dequeue();
        Debug.Log("Starting processing of card" + cardWaiting.card.cardName);
        if(CombatSystem.instance.acceptProcess) CheckCardProcess(animator);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (CombatSystem.instance.acceptProcess)  CheckCardProcess(animator);
    }

    public void CheckCardProcess(Animator animator)
    {
        animator.SetBool("CardsQueued", CombatSystem.instance.CardQueue.Count != 0);
        if (
            (cardWaiting.card.targetRequired && !CombatSystem.instance.EnemiesInScene.Contains((CombatActorEnemy)cardWaiting.suppliedTarget))
            ||
            (CombatSystem.instance.cEnergy < cardWaiting.card.displayCost))
        {
            CombatSystem.instance.Hand.Add(cardWaiting.card);
            cardWaiting.card.selectable = true;
            CombatSystem.instance.RefreshHandPositions();
            cardWaiting.card.animator.SetTrigger("Unplayable");
        }
        else
        {
            CombatSystem.instance.InProcessCard = cardWaiting.card;
            CombatSystem.instance.InProcessTarget = cardWaiting.suppliedTarget;
            CombatSystem.instance.cEnergy -= cardWaiting.card.displayCost;
            cardWaiting.card.animator.SetTrigger("CanPlay");
            animator.SetTrigger("CardCanProcess");
        }
    }

}
