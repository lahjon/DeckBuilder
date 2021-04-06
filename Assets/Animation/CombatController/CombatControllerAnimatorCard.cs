using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllerAnimatorCard : CombatControllerAnimator 
{
    public static Card card;
    public static CombatActor suppliedTarget;
    public static string layerName;
    public string nextState;

    public string nextLayerState { get { return layerName + "." + nextState; } }

    public void SetRefs(Animator animator)
    {
        base.SetRefs(animator);
        card = combatController.InProcessCard;
        suppliedTarget = combatController.InProcessTarget;
    }

}
