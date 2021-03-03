using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCombatAnimator : StateMachineBehaviour
{
    public static CombatController combatController;
    public CardCombatAnimated card;

    public void SetRefs(Animator animator)
    {
        if (combatController is null) 
            combatController = WorldSystem.instance.combatManager.combatController;
        if(card is null)
            card = animator.GetComponent<CardCombatAnimated>();
    }

    public Vector3 AngleLerp(Vector3 StartAngle, Vector3 FinishAngle, float t)
    {
        float xLerp = Mathf.LerpAngle(StartAngle.x, FinishAngle.x, t);
        float yLerp = Mathf.LerpAngle(StartAngle.y, FinishAngle.y, t);
        float zLerp = Mathf.LerpAngle(StartAngle.z, FinishAngle.z, t);
        return new Vector3(xLerp, yLerp, zLerp);
    }

    public (Vector3 pos, Vector3 scale, Vector3 angles) TransSnapshot()
    {
        return (card.transform.localPosition, card.transform.localScale, card.transform.localEulerAngles);
    }

    public void CardLerp((Vector3 pos,Vector3 scale ,Vector3 angles) startTransInfo, (Vector3 pos, Vector3 scale, Vector3 angles) targetTransInfo, float percent)
    {
        card.transform.localPosition    = Vector3.Lerp(startTransInfo.pos,      targetTransInfo.pos,    percent);
        card.transform.localScale       = Vector3.Lerp(startTransInfo.scale,    targetTransInfo.scale,  percent);
        card.transform.localEulerAngles = AngleLerp(   startTransInfo.angles,   targetTransInfo.angles, percent);

    }
}
