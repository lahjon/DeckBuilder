// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class ConditionWinCombat : Condition, IEvents
// {
//     public ConditionWinCombat(string aValue, int aAmount, Action aUpdateCondition, Action aCompleteCondition) : base(aValue, aAmount, aUpdateCondition, aCompleteCondition)
//     {
//     }

//     void WinCombat()
//     {
//         currentAmount++;
//         CheckCondition();
//     }

//     public override void Subscribe()
//     {
//         EventManager.OnWinCombatEvent += WinCombat;
//         Debug.Log("Sub");
//     }

//     public override void Unsubscribe()
//     {
//         EventManager.OnWinCombatEvent -= WinCombat;
//         Debug.Log("Desub");
//     }
// }