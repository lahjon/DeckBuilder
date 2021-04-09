using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllerAnimatorEnemyEnd: CombatControllerAnimator
{
    CombatActorEnemy enemy;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);

        enemy = (CombatActorEnemy)combatController.ActiveActor;
        combatController.StartCoroutine(EnemyEnd());
    }

    public IEnumerator EnemyEnd()
    {
        for (int i = 0; i < enemy.actionsEndTurn.Count; i++)
            yield return combatController.StartCoroutine(enemy.actionsEndTurn[i].Invoke());
        
        combatController.animator.SetBool("EnemyQueued", combatController.enemiesWaiting.Count != 0);
        combatController.animator.SetTrigger("EnemyFinished");
    }
 

}
