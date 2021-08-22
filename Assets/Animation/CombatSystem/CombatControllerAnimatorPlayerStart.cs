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
        hero = combat.ActiveActor = combat.Hero;
        combat.StartCoroutine(StartPlayerTurn());
        combat.combatOverlay.AnimatePlayerTurn();
    }


    public IEnumerator StartPlayerTurn()
    {
        yield return combat.StartCoroutine(RulesSystem.instance.StartTurn());
        
        for(int i = 0; i < hero.actionsNewTurn.Count; i++)
            yield return combat.StartCoroutine(hero.actionsNewTurn[i].Invoke());



        combat.acceptEndTurn = true;
        WorldSystem.instance.characterManager.characterVariablesUI.UpdateCharacterHUD();
        combat.EnemiesInScene.ForEach(x => x.DrawCard());
        combat.EnemiesInScene.ForEach(x => x.ShowMoveDisplay(true));

        combat.animator.SetTrigger("PlayerTurnStarted");
        yield return null;
    }


}
