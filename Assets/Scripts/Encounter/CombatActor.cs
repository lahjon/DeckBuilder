using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatActor : MonoBehaviour
{
    public HealthEffects healthEffects;
    public CombatController combatController;

    public List<Func<float, float>> dealAttackMods = new List<Func<float, float>>();
    public List<Func<float, float>> takeAttackMods = new List<Func<float, float>>();

    public List<Func<IEnumerator>> actionsNewTurn = new List<Func<IEnumerator>>();
    public List<Func<IEnumerator>> actionsEndTurn = new List<Func<IEnumerator>>();

    public List<Func<CombatActor, IEnumerator>> onAttackRecieved = new List<Func<CombatActor,IEnumerator>>();

    

    public void Awake()
    {
        actionsNewTurn.Add(RemoveAllBlock);
    }

    public IEnumerator RemoveAllBlock()
    {
        healthEffects.RemoveAllBlock();
        yield return new WaitForSeconds(1);
    }
}
