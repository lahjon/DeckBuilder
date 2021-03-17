using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllerAnimatorEnemyTurn : CombatControllerAnimator
{
    CombatActorEnemy enemy;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Entered enemy start state");
        SetRefs(animator);

        enemy = combatController.enemiesWaiting.Dequeue();
        combatController.ActiveActor = enemy;
        combatController.StartCoroutine(EnemyTurn());
    }

    /*
            foreach (CombatActorEnemy enemy in combatController.EnemiesInScene)
            yield return StartCoroutine(EnemyStartTurn(enemy))
    */

    public IEnumerator EnemyTurn()
    {
        WorldSystem.instance.characterManager.characterVariablesUI.UpdateCharacterHUD();
        yield return combatController.StartCoroutine(RulesSystem.instance.EnemyStartTurn(enemy));

        if (combatController.enemiesWaiting.Count == 0)
            combatController.animator.SetTrigger("EnemyTookTurn");
        else
            combatController.animator.SetTrigger("NextEnemy");
    }
 

}
