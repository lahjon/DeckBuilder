// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public abstract class WorldEncounterCondition
// {
//     int _currentAmount;
//     public int requiredAmount;
//     public string value;
//     public int currentAmount
//     {
//         get => _currentAmount;
//         set
//         {
//             if (item != null && !item.usable)
//                 _currentAmount = value;
//         }
//     }
//     public WorldEncounterCondition(string aValue, int aAmount)
//     {
//         item = anItem;
//         value = aValue;
//         requiredAmount = aAmount;
//         Subscribe();
//     }

//     public abstract void Subscribe();
//     public abstract void Unsubscribe();

//     public void CheckCondition()
//     {
        
//         if (currentAmount >= requiredAmount)
//         {
//             item.counterText.text = "";
//             currentAmount = 0;
//             item.charges++;
//         }
//         else
//         {
//             UpdateCounter();
//         }
//     }

//     public void UpdateCounter()
//     {
//         item.counterText.text = (requiredAmount - currentAmount).ToString();
//     }

//     public static WorldEncounterCondition GetCondition(ConditionStruct type, UseItem item) => type.type switch
//     {
//         ConditionType.KillEnemy => new ItemConditionKillEnemy(type.value, type.amount, item),
//         ConditionType.ClearTile => new ItemConditionCompleteTile(type.value, type.amount, item),
//         ConditionType.WinCombat => new ItemConditionWinCombat(type.value, type.amount, item),
//         _                           => null,
//     };
//     public static string GetDescription(ConditionStruct type) => type.type switch
//     {
//         ConditionType.KillEnemy => string.Format("<b>Kill Enemies (" + int.Parse(type.value) + ")</b>"),
//         ConditionType.ClearTile => string.Format("<b>Clear Tiles (" + int.Parse(type.value) + ")</b>"),
//         ConditionType.WinCombat => string.Format("<b>Win Combats (" + int.Parse(type.value) + ")</b>"),
//         _                           => null,
//     };



// }
