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
        cardWaiting = combatController.CardQueue.Dequeue();

        if(combatController.acceptProcess) CheckCardProcess(animator);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (combatController.acceptProcess)  CheckCardProcess(animator);
    }

    public void CheckCardProcess(Animator animator)
    {
        animator.SetBool("CardsQueued", combatController.CardQueue.Count != 0);
        if (
            (cardWaiting.card.targetRequired && !combatController.EnemiesInScene.Contains((CombatActorEnemy)cardWaiting.suppliedTarget))
            ||
            (combatController.cEnergy < cardWaiting.card.displayCost))
        {
            combatController.Hand.Add(cardWaiting.card);
            combatController.RefreshHandPositions();
            cardWaiting.card.animator.SetTrigger("Unplayable");
        }
        else
        {
            combatController.InProcessCard = cardWaiting.card;
            combatController.InProcessTarget = cardWaiting.suppliedTarget;
            combatController.cEnergy -= cardWaiting.card.displayCost;
            cardWaiting.card.animator.SetTrigger("CanPlay");
            animator.SetTrigger("CardCanProcess");
        }
    }

}
