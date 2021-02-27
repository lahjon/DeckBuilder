using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllerAnimatorEnemyTurn : CombatControllerAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Entered enemy start state");
        SetRefs(animator);
        animator.SetBool("EnemiesWaiting", true);
        combatController.StartCoroutine(EnemyTurn());
    }

    public IEnumerator EnemyTurn()
    {
        WorldSystem.instance.characterManager.characterVariablesUI.UpdateUI();
        RulesSystem.instance.EnemiesStartTurn();

        yield return null;
    }
 

}
