using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatActorHero : CombatActor
{
    private void Start()
    {
        healthEffects.combatActor = this;
    }
}
