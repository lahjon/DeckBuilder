using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllerAnimatorPlayerStart : CombatControllerAnimator
{
    CombatActor hero;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);
        Debug.Log("Entered Player Start");
        combatController.ActiveActor = combatController.Hero;
        hero = combatController.Hero;
        combatController.StartCoroutine(StartPlayerTurn());
    }


    public IEnumerator StartPlayerTurn()
    {
        yield return combatController.StartCoroutine(RulesSystem.instance.StartTurn());
        for(int i = 0; i < hero.actionsNewTurn.Count; i++)
            yield return combatController.StartCoroutine(hero.actionsNewTurn[i].Invoke());

        hero.EffectsOnNewTurnBehavior();

        combatController.acceptEndTurn = true;
        WorldSystem.instance.characterManager.characterVariablesUI.UpdateCharacterHUD();
        combatController.EnemiesInScene.ForEach(x => x.DrawCard());
        combatController.EnemiesInScene.ForEach(x => x.ShowMoveDisplay(true));

        combatController.animator.SetTrigger("PlayerTurnStarted");
        yield return null;
    }


}
