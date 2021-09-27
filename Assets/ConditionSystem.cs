using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class ConditionSystem : MonoBehaviour
{
    public ConditionSystem instance; 

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static Func<ConditionStruct, bool> GetConditionChecker(ConditionType type)
    {
        switch (type)
        {
            case ConditionType.CardsPlayedAtLeast:
                return CheckCardsPlayedAtLeast;
            case ConditionType.CardsPlayedAtMost:
                return CheckCardsPlayedAtMost;
            case ConditionType.LastCardPlayedTurnType:
                return CheckLastTypePlayedThisTurn;
            default:
                return null;
        }
    }

    public static bool CheckCardsPlayedAtLeast(ConditionStruct conditionStruct)
    {
        Debug.Log("cards played vs value:" + CombatSystem.instance.cardsPlayedThisTurn.Count + "," + conditionStruct.numValue);
        return CombatSystem.instance.cardsPlayedThisTurn.Count >= conditionStruct.numValue;
    }

    public static bool CheckCardsPlayedAtMost(ConditionStruct conditionStruct)
    {
        return CombatSystem.instance.cardsPlayedThisTurn.Count <= conditionStruct.numValue;
    }

    public static bool CheckLastTypePlayedThisTurn(ConditionStruct conditionStruct)
    {
        if (CombatSystem.instance.cardsPlayedThisTurn.Count < 1) return false;
        CardType cardType;
        Enum.TryParse(conditionStruct.strParameter, out cardType);
        return CombatSystem.instance.cardsPlayedThisTurn[CombatSystem.instance.cardsPlayedThisTurn.Count -1].cardType == cardType;
    }


}
