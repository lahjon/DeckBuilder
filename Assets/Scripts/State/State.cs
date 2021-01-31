using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    protected CombatController CombatController;
    // protected RulesSystem RulesSystem;

    public State(CombatController combatController)
    {
        CombatController = combatController;
    }

    // public State(RulesSystem rulesSystem)
    // {
    //     RulesSystem = rulesSystem;
    // }
    public virtual IEnumerator Start()
    {
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
