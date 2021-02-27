using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllerAnimatorPlayerStart : CombatControllerAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);
        combatController.StartCoroutine(StartPlayerTurn());
    }


    public IEnumerator StartPlayerTurn()
    {
        combatController.StartTurn();
        combatController.acceptSelections = true;
        WorldSystem.instance.characterManager.characterVariablesUI.UpdateUI();
        WorldSystem.instance.combatManager.combatController.EnemiesInScene.ForEach(x => x.healthEffects.EffectsOnNewTurnBehavior());

        combatController.animator.SetTrigger("PlayerTurnStarted");
        yield return null;
    }


}
