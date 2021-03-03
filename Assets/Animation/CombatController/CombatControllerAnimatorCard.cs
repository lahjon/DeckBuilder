using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllerAnimatorCard : CombatControllerAnimator 
{
    public static CardData card;
    public static CombatActor suppliedTarget;
    public readonly string layerName = "Resolve Card";
    public string nextState;

    public string nextLayerState { get { return layerName + "." + nextState; } }

    public void SetRefs(Animator animator)
    {
        base.SetRefs(animator);
        card = combatController.CardInProcess.card;
        suppliedTarget = combatController.CardInProcess.target;

    }

}
