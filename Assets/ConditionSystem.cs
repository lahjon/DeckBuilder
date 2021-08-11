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


    public static bool CheckCondition(ConditionStruct conditionStruct)
    {
        switch (conditionStruct.type)
        {
            case ConditionType.CardsPlayedAtLeast:
                return CheckCardsPlayeAtLeast(conditionStruct.value);
            case ConditionType.CardsPlayedAtMost:
                return CheckCardsPlayedAtMost(conditionStruct.value);
            case ConditionType.LastCardPlayedTurnType:
                return CheckLastTypePlayedThisTurn(conditionStruct.value);
            default:
                return false;
        }
    }

    public static Func<string, bool> GetConditionChecker(ConditionType type)
    {
        switch (type)
        {
            case ConditionType.CardsPlayedAtLeast:
                return CheckCardsPlayeAtLeast;
            case ConditionType.CardsPlayedAtMost:
                return CheckCardsPlayedAtMost;
            case ConditionType.LastCardPlayedTurnType:
                return CheckLastTypePlayedThisTurn;
            default:
                return null;
        }
    }

    public static bool CheckCardsPlayeAtLeast(string nrLimit)
    {
        return CombatSystem.instance.cardsPlayedThisTurn.Count >= int.Parse(nrLimit);
    }

    public static bool CheckCardsPlayedAtMost(string nrLimit)
    {
        return CombatSystem.instance.cardsPlayedThisTurn.Count <= int.Parse(nrLimit);
    }

    public static bool CheckLastTypePlayedThisTurn(string TypeName)
    {
        if (CombatSystem.instance.cardsPlayedThisTurn.Count < 1) return false;
        CardType cardType;
        Enum.TryParse(TypeName, out cardType);
        return CombatSystem.instance.cardsPlayedThisTurn[CombatSystem.instance.cardsPlayedThisTurn.Count -1].cardType == cardType;
    }


}
