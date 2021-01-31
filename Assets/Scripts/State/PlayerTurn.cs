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
        CombatController.InitializeCombat();
        yield return new WaitForSeconds(1.0f);
    }

    public override IEnumerator Action()
    {
        yield return new WaitForSeconds(1.0f);
    }
}