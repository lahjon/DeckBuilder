using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatActorHero : CombatActor
{
    void Start()
    {
        this.GetComponent<SpriteRenderer>().sprite = WorldSystem.instance.characterManager.character.characterData.artwork;
        healthEffects.combatActor = this;
        healthEffects.maxHitPoints =  WorldSystem.instance.characterManager.characterStats.GetStat(StatType.Health);
        healthEffects.hitPoints =  WorldSystem.instance.characterManager.currentHealth;
    }


    void OnEnable()
    {

    }
}
