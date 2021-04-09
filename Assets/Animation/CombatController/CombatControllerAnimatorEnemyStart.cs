using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllerAnimatorEnemyStart: CombatControllerAnimator
{
    CombatActorEnemy enemy;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Entered enemy start state");
        SetRefs(animator);

        enemy = combatController.enemiesWaiting.Dequeue();
        combatController.ActiveActor = enemy;
        combatController.InProcessCard = enemy.hand;
        combatController.InProcessTarget = combatController.Hero;
        combatController.StartCoroutine(EnemyTurn());
    }

    /*
            foreach (CombatActorEnemy enemy in combatController.EnemiesInScene)
            yield return StartCoroutine(EnemyStartTurn(enemy))
    */

    public IEnumerator EnemyTurn()
    {
        WorldSystem.instance.characterManager.characterVariablesUI.UpdateCharacterHUD();
        enemy.ShowMoveDisplay(false);
        for (int i = 0; i < enemy.actionsNewTurn.Count; i++)
            yield return combatController.StartCoroutine(enemy.actionsNewTurn[i].Invoke());

        enemy.EffectsOnNewTurnBehavior();
        combatController.animator.SetTrigger("EnemyPlayCard");
    }
 

}
