using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCombatAnimatorQueued : CardCombatAnimator
{

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);
        CombatSystem.instance.Hand.Remove(card);
        card.selectable = false;
        card.image.raycastTarget = false;
        CombatSystem.instance.RefreshHandPositions();
        card.transform.localEulerAngles = Vector3.zero;

        CombatSystem.instance.CardQueue.Enqueue((card, CombatSystem.instance.TargetedEnemy));
        CombatSystem.instance.HeroCardsWaiting.Enqueue(card);
        CombatSystem.instance.animator.SetBool("CardsQueued", true);
    }


}

