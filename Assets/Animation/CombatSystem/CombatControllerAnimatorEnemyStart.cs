using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllerAnimatorEnemyStart: CombatControllerAnimator
{
    CombatActorEnemy enemy;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("Entered enemy start state");
        SetRefs(animator);

        combat.combatOverlay.AnimateEnemyTurn();
        combat.actorTurn = CombatActorTypes.Enemy;

        combat.StartCoroutine(EnemyTurn());
    }


    public IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(.5f);
        combat.animator.SetTrigger("EnemyTurnStart");
    }
 

}
