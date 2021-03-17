using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitCombat : State
{
    public ExitCombat(CombatController combatController) : base(combatController)
    {
    }

    public override IEnumerator Start()
    {
        Debug.Log("HEJ");
        WorldSystem.instance.characterManager.characterVariablesUI.UpdateCharacterHUD();
        Debug.Log("Starting combat");
        RulesSystem.instance.SetupEnemyStartingRules();
        yield return new WaitForSeconds(0.5f);
        Debug.Log("Combat has Started");
        //CombatController.InitializeCombat();
        //CombatController.SetState(new PlayerTurn(CombatController));
    }
}

