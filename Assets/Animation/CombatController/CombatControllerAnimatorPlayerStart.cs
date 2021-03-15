using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllerAnimatorPlayerStart : CombatControllerAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);
        Debug.Log("Entered Player Start");
        combatController.ActiveActor = combatController.Hero;
        combatController.StartCoroutine(StartPlayerTurn());
    }


    public IEnumerator StartPlayerTurn()
    {                 
        combatController.StartTurn();
        combatController.acceptSelections = true;
        WorldSystem.instance.characterManager.characterVariablesUI.UpdateUI();
        combatController.EnemiesInScene.ForEach(x => x.healthEffects.EffectsOnNewTurnBehavior());
        combatController.EnemiesInScene.ForEach(x => x.DrawCard());
        combatController.EnemiesInScene.ForEach(x => x.ShowMoveDisplay(true));

        combatController.animator.SetTrigger("PlayerTurnStarted");
        yield return null;
    }


}
