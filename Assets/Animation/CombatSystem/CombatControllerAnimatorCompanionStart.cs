using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllerAnimatorCompanionStart: CombatControllerAnimator
{
    CombatActorCompanion companion;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Entered companion start state");
        SetRefs(animator);
        companion = combat.companion;
        combat.actorTurn = CombatActorTypes.Companion;

        // combat.combatOverlay.AnimateEnemyTurn();

        // enemy = combat.enemiesWaiting.Dequeue();
        //combat.ActiveActor = companion;
        combat.InProcessCard = companion.hand;
        combat.InProcessTarget = combat.EnemiesInScene[0];
        combat.StartCoroutine(CompanionTurn());
    }


    public IEnumerator CompanionTurn()
    {
        companion.ShowMoveDisplay(false);
        for (int i = 0; i < companion.actionsNewTurn.Count; i++)
            yield return combat.StartCoroutine(companion.actionsNewTurn[i].Invoke());

        combat.animator.SetTrigger("CompanionPlayCard");
    }
 

}
