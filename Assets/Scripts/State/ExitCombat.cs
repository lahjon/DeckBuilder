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
        WorldSystem.instance.characterManager.characterVariablesUI.UpdateUI();
        Debug.Log("Starting combat");
        yield return new WaitForSeconds(0.5f);
        Debug.Log("Combat has Started");
        //CombatController.InitializeCombat();
        //CombatController.SetState(new PlayerTurn(CombatController));
    }
}

