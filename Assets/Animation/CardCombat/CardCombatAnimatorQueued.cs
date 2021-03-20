using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCombatAnimatorQueued : CardCombatAnimator
{

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);
        combatController.Hand.Remove(card);
        card.selectable = false;
        combatController.RefreshHandPositions();
        card.transform.localEulerAngles = Vector3.zero;

        combatController.CardQueue.Enqueue((card, combatController.ActiveEnemy));
        combatController.HeroCardsWaiting.Enqueue(card);
        combatController.animator.SetBool("CardsQueued", true);
    }


}

