using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllerAnimatorPlayerStart : CombatControllerAnimator
{
    CombatActor hero;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);
        //Debug.Log("Entered Player Start");
        CombatSystem.instance.ActiveActor = CombatSystem.instance.Hero;
        hero = CombatSystem.instance.Hero;
        CombatSystem.instance.StartCoroutine(StartPlayerTurn());
        CombatSystem.instance.combatOverlay.AnimatePlayerTurn();
    }


    public IEnumerator StartPlayerTurn()
    {
        yield return CombatSystem.instance.StartCoroutine(RulesSystem.instance.StartTurn());
        for(int i = 0; i < hero.actionsNewTurn.Count; i++)
            yield return CombatSystem.instance.StartCoroutine(hero.actionsNewTurn[i].Invoke());



        hero.EffectsOnNewTurnBehavior();

        CombatSystem.instance.acceptEndTurn = true;
        WorldSystem.instance.characterManager.characterVariablesUI.UpdateCharacterHUD();
        CombatSystem.instance.EnemiesInScene.ForEach(x => x.DrawCard());
        CombatSystem.instance.EnemiesInScene.ForEach(x => x.ShowMoveDisplay(true));

        CombatSystem.instance.animator.SetTrigger("PlayerTurnStarted");
        yield return null;
    }


}
