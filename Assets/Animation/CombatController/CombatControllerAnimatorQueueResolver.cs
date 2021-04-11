using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllerAnimatorQueueResolver : CombatControllerAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);

        (CardCombat card, CombatActor suppliedTarget) cardWaiting = combatController.CardQueue.Dequeue();

        animator.SetBool("CardsQueued", combatController.CardQueue.Count != 0);

        if (
            (cardWaiting.card.targetRequired && !combatController.EnemiesInScene.Contains((CombatActorEnemy)cardWaiting.suppliedTarget)) 
            ||
            (combatController.cEnergy < cardWaiting.card.cost))
        {
            combatController.Hand.Add(cardWaiting.card);
            combatController.RefreshHandPositions();
            cardWaiting.card.animator.SetTrigger("Unplayable");
        }
        else
        {
            combatController.InProcessCard = cardWaiting.card;
            combatController.InProcessTarget = cardWaiting.suppliedTarget;
            combatController.cEnergy -= cardWaiting.card.cost;
            cardWaiting.card.animator.SetTrigger("CanPlay");
            animator.SetTrigger("CardCanProcess");
        }
    }


}
