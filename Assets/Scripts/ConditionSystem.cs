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

    public static Func<ConditionData, bool> GetConditionChecker(ConditionType type)
    {
        switch (type)
        {
            case ConditionType.CardsPlayedAtLeast:
                return CheckCardsPlayedAtLeast;
            case ConditionType.CardsPlayedAtMost:
                return CheckCardsPlayedAtMost;
            case ConditionType.LastCardPlayedTurnType:
                return CheckLastTypePlayedThisTurn;
            case ConditionType.OptionalCardCostPaid:
                return CheckOptionalCardCostPaid;
            default:
                return null;
        }
    }

    public static bool CheckCardsPlayedAtLeast(ConditionData conditionStruct)
    {
        return CombatSystem.instance.cardsPlayedThisTurn.Count >= conditionStruct.numValue;
    }

    public static bool CheckCardsPlayedAtMost(ConditionData conditionStruct)
    {
        return CombatSystem.instance.cardsPlayedThisTurn.Count <= conditionStruct.numValue;
    }

    public static bool CheckLastTypePlayedThisTurn(ConditionData conditionStruct)
    {
        if (CombatSystem.instance.cardsPlayedThisTurn.Count < 1) return false;
        CardType cardType;
        Enum.TryParse(conditionStruct.strParameter, out cardType);
        return CombatSystem.instance.cardsPlayedThisTurn[CombatSystem.instance.cardsPlayedThisTurn.Count -1].cardType == cardType;
    }

    public static bool CheckOptionalCardCostPaid(ConditionData conditionStruct)
    {
        return CombatSystem.instance.InProcessCard is Card card ? card.cost.optionalPaid : false;
    }
}
