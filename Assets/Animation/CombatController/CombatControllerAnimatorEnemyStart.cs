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

        combatController.combatOverlay.AnimateEnemyTurn();

        enemy = combatController.enemiesWaiting.Dequeue();
        combatController.ActiveActor = enemy;
        combatController.InProcessCard = enemy.hand;
        combatController.InProcessTarget = combatController.Hero;
        combatController.StartCoroutine(EnemyTurn());
    }


    public IEnumerator EnemyTurn()
    {
        WorldSystem.instance.characterManager.characterVariablesUI.UpdateCharacterHUD();
        enemy.ShowMoveDisplay(false);
        for (int i = 0; i < enemy.actionsNewTurn.Count; i++)
            yield return combatController.StartCoroutine(enemy.actionsNewTurn[i].Invoke());

        combatController.animator.SetTrigger("EnemyPlayCard");
    }
 

}
