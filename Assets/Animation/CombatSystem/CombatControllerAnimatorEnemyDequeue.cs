using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllerAnimatorEnemyDequeue: CombatControllerAnimator
{
    CombatActorEnemy enemy;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("Entered enemy start state");
        SetRefs(animator);

        enemy = combat.enemiesWaiting.Dequeue();
        combat.ActiveActor = enemy;
        combat.InProcessCard = enemy.hand;
        combat.InProcessTarget = combat.Hero;
        combat.StartCoroutine(EnemyTurn());
    }


    public IEnumerator EnemyTurn()
    {
        enemy.ShowMoveDisplay(false);
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < enemy.actionsNewTurn.Count; i++)
            yield return combat.StartCoroutine(enemy.actionsNewTurn[i].Invoke());

        combat.animator.SetTrigger("EnemyPlayCard");
    }
 

}
