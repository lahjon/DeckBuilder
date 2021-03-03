using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllerAnimatorQueueResolver : CombatControllerAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);

        (CardData card, CombatActor suppliedTarget) cardWaiting = combatController.CardQueue.Dequeue();
        CardCombatAnimated cardCombat = combatController.HeroCardsWaiting.Dequeue();

        animator.SetBool("CardsQueued", combatController.CardQueue.Count != 0);

        if (
            (cardWaiting.card.targetRequired && !combatController.EnemiesInScene.Contains((CombatActorEnemy)cardWaiting.suppliedTarget)) 
            ||
            (combatController.cEnergy < cardWaiting.card.cost))
        {
            cardCombat.animator.SetTrigger("Unplayable");
        }
        else
        {
            combatController.CardInProcess = cardWaiting;
            combatController.HeroCardInProcess = cardCombat;
            animator.SetTrigger("CardCanProcess");
        }
    }


}
