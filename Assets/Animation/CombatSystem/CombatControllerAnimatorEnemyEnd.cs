using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllerAnimatorEnemyEnd: CombatControllerAnimator
{
    CombatActorEnemy enemy;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);

        enemy = (CombatActorEnemy)CombatSystem.instance.ActiveActor;
        CombatSystem.instance.StartCoroutine(EnemyEnd());
    }

    public IEnumerator EnemyEnd()
    {
        for (int i = 0; i < enemy.actionsEndTurn.Count; i++)
            yield return CombatSystem.instance.StartCoroutine(enemy.actionsEndTurn[i].Invoke());

        yield return CombatSystem.instance.StartCoroutine(enemy.EffectsOnEndTurnBehavior());
        
        CombatSystem.instance.animator.SetBool("EnemyQueued", CombatSystem.instance.enemiesWaiting.Count != 0);
        CombatSystem.instance.animator.SetTrigger("EnemyFinished");
    }
 

}
