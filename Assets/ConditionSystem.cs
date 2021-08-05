using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            case ConditionType.CardsPlayedAbove:
                return CombatSystem.instance.cardsPlayedThisTurn.Count > int.Parse(conditionStruct.value);
            case ConditionType.CardsPlayedBelow:
                return CombatSystem.instance.cardsPlayedThisTurn.Count < int.Parse(conditionStruct.value);
            default:
                return false;
        }
    }




}
