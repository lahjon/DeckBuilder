using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCombatAnimatorDeck : CardCombatAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);
        card.transform.position = combatController.txtDeck.transform.position;
        card.transform.localScale = Vector3.zero;
        card.transform.localEulerAngles = Vector3.zero;
        card.MouseReact = false;
        card.selectable = false;
    }

}

