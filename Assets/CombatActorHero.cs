using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatActorHero : CombatActor
{
    void Start()
    {
        healthEffects.combatActor = this;
        this.GetComponent<SpriteRenderer>().sprite = WorldSystem.instance.characterManager.characterData.artwork;
    }

    void OnEnable()
    {

    }
}
