using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCombatAnimator : StateMachineBehaviour
{
    public CardCombatAnimated card;
    public CombatController combatController;

    public void SetRefs(Animator animator)
    {
        if(card is null)
        {
            card = animator.GetComponent<CardCombatAnimated>();
            combatController = card.combatController;
        }
    }

    public Vector3 AngleLerp(Vector3 StartAngle, Vector3 FinishAngle, float t)
    {
        float xLerp = Mathf.LerpAngle(StartAngle.x, FinishAngle.x, t);
        float yLerp = Mathf.LerpAngle(StartAngle.y, FinishAngle.y, t);
        float zLerp = Mathf.LerpAngle(StartAngle.z, FinishAngle.z, t);
        return new Vector3(xLerp, yLerp, zLerp);
    }


}
