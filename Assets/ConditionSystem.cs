using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
        Debug.Log("Checking for type and para" + conditionStruct.type.ToString() + conditionStruct.value);
        Debug.Log(CombatSystem.instance.cardsPlayedThisTurn.Count);
        Debug.Log(int.Parse(conditionStruct.value));


        switch (conditionStruct.type)
        {
            case ConditionType.CardsPlayedAbove:
                return CombatSystem.instance.cardsPlayedThisTurn.Count > int.Parse(conditionStruct.value);
            case ConditionType.CardsPlayedBelow:
                return CombatSystem.instance.cardsPlayedThisTurn.Count < int.Parse(conditionStruct.value);
            default:
                return false;
        }
    }

    public static Func<string, bool> GetConditionChecker(ConditionType type)
    {
        switch (type)
        {
            case ConditionType.CardsPlayedAbove:
                return CheckCardsPlayedAbove;
            case ConditionType.CardsPlayedBelow:
                return CheckCardsPlayedBelow;
            default:
                return null;
        }
    }

    public static bool CheckCardsPlayedAbove(string parameter)
    {
        return CombatSystem.instance.cardsPlayedThisTurn.Count > int.Parse(parameter);
    }

    public static bool CheckCardsPlayedBelow(string parameter)
    {
        return CombatSystem.instance.cardsPlayedThisTurn.Count < int.Parse(parameter);
    }



}
