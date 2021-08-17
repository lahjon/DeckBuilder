// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class ConditionCompleteTile : Condition, IEvents
// {
//     public ConditionCompleteTile(string aValue, int aAmount, Action aUpdateCondition, Action aCompleteCondition) : base(aValue, aAmount, aUpdateCondition, aCompleteCondition)
//     {
//     }

//     void CompleteTile()
//     {
//         currentAmount++;
//         CheckCondition();
//     }
//     public override void Subscribe()
//     {
//         EventManager.OnCompleteTileEvent += CompleteTile;
//     }

//     public override void Unsubscribe()
//     {
//         EventManager.OnCompleteTileEvent -= CompleteTile;
//     }
// }
