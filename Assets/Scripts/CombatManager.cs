using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : Manager
{
    public CombatController combatController;

    protected override void Awake()
    {
        base.Awake(); 
        world.combatManager = this;
    }
}
