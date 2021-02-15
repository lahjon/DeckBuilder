using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State 
{
    protected CombatController CombatController;
    public State(CombatController combatController)
    {
        CombatController = combatController;
    }
    public virtual IEnumerator Start()
    {
        WorldSystem.instance.characterManager.characterVariablesUI.UpdateUI();
        yield break;
    }
    public virtual IEnumerator End()
    {
        yield break;
    }
    public virtual IEnumerator Action()
    {
        yield break;
    }
}
