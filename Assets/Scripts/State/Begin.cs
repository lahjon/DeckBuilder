using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Begin : State
{
    public Begin(CombatController combatController) : base(combatController)
    {
    }

    // public Begin(RulesSystem rulesSystem) : base(rulesSystem)
    // {
    // }

    public override IEnumerator Start()
    {
        Debug.Log("Starting combat");
        RulesSystem.instance.SetupEnemyStartingRules();
        yield return new WaitForSeconds(1.0f);
        Debug.Log("Combat has Started");
        CombatController.SetState(new PlayerTurn(CombatController));

    }
}

