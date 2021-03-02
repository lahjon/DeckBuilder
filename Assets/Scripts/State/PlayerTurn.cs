using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurn : State
{
    public PlayerTurn(CombatController combatController) : base(combatController)
    {
    }

    public override IEnumerator Start()
    {
        CombatController.StartTurn();
        CombatController.acceptSelections = true;
        WorldSystem.instance.characterManager.characterVariablesUI.UpdateUI();
        WorldSystem.instance.combatManager.combatController.EnemiesInScene.ForEach(x => x.healthEffects.EffectsOnNewTurnBehavior());

        yield return new WaitForSeconds(0.05f);
    }

    public override IEnumerator Action()
    {
        yield return new WaitForSeconds(0.05f);
    }
    public override IEnumerator End()
    {
        CombatController.acceptSelections = false;
        CombatController.EndTurn();
        yield return new WaitForSeconds(0.05f);
        //CombatController.SetState(new EnemyTurn(CombatController));
    }
}

