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
        combatController.CardInProcess = (enemy.hand, combatController.Hero);
        combatController.StartCoroutine(EnemyTurn());
    }

    /*
            foreach (CombatActorEnemy enemy in combatController.EnemiesInScene)
            yield return StartCoroutine(EnemyStartTurn(enemy))
    */

    public IEnumerator EnemyTurn()
    {
        WorldSystem.instance.characterManager.characterVariablesUI.UpdateUI();
        enemy.ShowMoveDisplay(false);
        yield return combatController.StartCoroutine(RulesSystem.instance.EnemyStartTurn(enemy));

        combatController.animator.SetTrigger("EnemyPlayCard");
    }
 

}
