using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllerAnimatorEnemyEnd: CombatControllerAnimator
{
    CombatActorEnemy enemy;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);

        enemy = (CombatActorEnemy)combat.ActiveActor;
        combat.StartCoroutine(EnemyEnd());
    }

    public IEnumerator EnemyEnd()
    {
        for (int i = 0; i < enemy.actionsEndTurn.Count; i++)
            yield return combat.StartCoroutine(enemy.actionsEndTurn[i].Invoke());

        yield return combat.StartCoroutine(enemy.EffectsOnEndTurnBehavior());
        
        combat.animator.SetBool("EnemyQueued", combat.enemiesWaiting.Count != 0);
        combat.animator.SetTrigger("EnemyFinished");
    }
 

}
