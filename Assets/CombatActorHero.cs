using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatActorHero : MonoBehaviour
{
    private HealthEffects healthEffects;

    public void Start()
    {
        healthEffects = GetComponentInChildren<HealthEffects>();
    }
}
