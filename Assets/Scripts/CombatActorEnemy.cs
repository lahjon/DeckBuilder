using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CombatActorEnemy : MonoBehaviour
{
    public CombatController combatController;
    public HealthEffects healthEffects;

    public void Start()
    {
        healthEffects = GetComponentInChildren<HealthEffects>();
    }



    public void OnMouseOver()
    {
        if (combatController.ActiveEnemy is null) combatController.ActiveEnemy = this;
    }

    public void OnMouseExit()
    {
        if (combatController.ActiveEnemy == this) combatController.ActiveEnemy = null;
    }

    public void OnMouseDown()
    {
        combatController.EnemyClicked(this);
    }
}
