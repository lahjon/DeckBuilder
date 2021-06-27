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

        CombatSystem.instance.combatOverlay.AnimateEnemyTurn();

        enemy = CombatSystem.instance.enemiesWaiting.Dequeue();
        CombatSystem.instance.ActiveActor = enemy;
        CombatSystem.instance.InProcessCard = enemy.hand;
        CombatSystem.instance.InProcessTarget = CombatSystem.instance.Hero;
        CombatSystem.instance.StartCoroutine(EnemyTurn());
    }


    public IEnumerator EnemyTurn()
    {
        WorldSystem.instance.characterManager.characterVariablesUI.UpdateCharacterHUD();
        enemy.ShowMoveDisplay(false);
        for (int i = 0; i < enemy.actionsNewTurn.Count; i++)
            yield return CombatSystem.instance.StartCoroutine(enemy.actionsNewTurn[i].Invoke());

        CombatSystem.instance.animator.SetTrigger("EnemyPlayCard");
    }
 

}
