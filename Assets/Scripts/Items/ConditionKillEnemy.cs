// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class ConditionKillEnemy : Condition, IEvents
// {
//     public ConditionKillEnemy(string aValue, int aAmount, Action aUpdateCondition, Action aCompleteCondition) : base(aValue, aAmount, aUpdateCondition, aCompleteCondition)
//     {
//     }

//     void EnemyKilled(EnemyData enemy)
//     {
//         Debug.Log("Enemy Killed " + this);
//         if (enemy.enemyId == value || string.IsNullOrEmpty(value))
//         {
//             currentAmount++;
//             CheckCondition();
//         }
//     }
//     public override void Subscribe()
//     {
//         EventManager.OnEnemyKilledEvent += EnemyKilled;
//     }

//     public override void Unsubscribe()
//     {
//         EventManager.OnEnemyKilledEvent -= EnemyKilled;
//     }
// }
