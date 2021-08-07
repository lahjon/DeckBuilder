using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class CardCombatAnimatorDeck : CardCombatAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);
        card.Unsubscribe();
        card.transform.position = CombatSystem.instance.txtDeck.transform.position;
        card.transform.localScale = Vector3.zero;
        card.transform.localEulerAngles = Vector3.zero;
        card.MouseReact = false;
        card.selectable = false;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        card.Subscribe();
    }

}

