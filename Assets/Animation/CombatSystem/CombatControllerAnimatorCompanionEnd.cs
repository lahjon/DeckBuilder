using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllerAnimatorCompanionEnd: CombatControllerAnimator
{
    CombatActorCompanion companion;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);
        companion = combat.companion;
        Debug.Log("Companion End");
        combat.StartCoroutine(CompanionEnd());
    }

    public IEnumerator CompanionEnd()
    {
        for (int i = 0; i < companion.actionsEndTurn.Count; i++)
            yield return combat.StartCoroutine(companion.actionsEndTurn[i].Invoke());

        yield return combat.StartCoroutine(companion.EffectsOnEndTurnBehavior());
        combat.animator.SetTrigger("CompanionTurnEnded");
        if (combat.animator.GetBool("CompanionTurnStartedByPlayer")) combat.actorTurn = CombatActorType.Hero;
    }
 

}
