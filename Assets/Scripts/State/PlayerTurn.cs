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
        CombatController.acceptInput = true;
        WorldSystem.instance.characterManager.characterVariablesUI.UpdateUI();
        yield return new WaitForSeconds(0.05f);
    }

    public override IEnumerator Action()
    {
        yield return new WaitForSeconds(0.05f);
    }
    public override IEnumerator End()
    {
        CombatController.acceptInput = false;
        CombatController.EndTurn();
        yield return new WaitForSeconds(0.05f);
        CombatController.SetState(new EnemyTurn(CombatController));
    }
}

