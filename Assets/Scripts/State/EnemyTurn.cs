using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurn : State
{
    public EnemyTurn(CombatController combatController) : base(combatController)
    {
    }

    public override IEnumerator Start()
    {
        WorldSystem.instance.characterManager.characterVariablesUI.UpdateUI();
        RulesSystem.instance.EnemiesStartTurn();
        yield return null;
    }
    public override IEnumerator Action()
    {
        yield return new WaitForSeconds(0.05f);
    }
    public override IEnumerator End()
    {
        yield return new WaitForSeconds(0.05f);
        //CombatController.SetState(new PlayerTurn(CombatController));
    }
}