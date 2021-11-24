using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class CardActivitySystem : MonoBehaviour
{
    public static CardActivitySystem instance = null;
    public static Dictionary<CardActivityType, CardActivity> ActivityTypeToAction = new Dictionary<CardActivityType, CardActivity>
    {
        { CardActivityType.DrawCard, new CardActivityDrawCard()},
        { CardActivityType.AddCardToCombat, new CardActivityAddCardToCombat()},
        { CardActivityType.Splice, new CardActivitySplice()},
        { CardActivityType.ExhaustDiscard, new CardActivityExhaustDiscard()},
        { CardActivityType.CombatCostChange, new CardActivityCombatCostChange()},
        { CardActivityType.SetRandomBroken, new CardActivitySetRandomBroken()},
        { CardActivityType.ModifyLife, new CardActivityModifyLife()},
        { CardActivityType.ModifyEnergy, new CardActivityModifyEnergy()},
        { CardActivityType.DrawDiscard, new CardActivityDrawDiscard()},
        { CardActivityType.DualWield, new CardActivityDualWield()},
        { CardActivityType.EmptySavedEnergy, new CardActivityEmptySavedEnergy()},
        { CardActivityType.EN_AddCardModifier, new CardActivityEN_AddCardModifier()},
    };


    private void Awake()
    {
        if (instance != null) Destroy(gameObject);
        else instance = this;
        CardActivity.ActivitySystem = this;
    }

    public IEnumerator StartByCardActivity(CardActivitySetting cardActivity)
    {
        //Debug.Log("Starting card Activity:" + cardActivity.type);
        if (!ActivityTypeToAction.ContainsKey(cardActivity.type))
        {
            Debug.LogError("No function exists for Activity of type" + cardActivity.type.ToString());
        }
        else
            yield return StartCoroutine(ActivityTypeToAction[cardActivity.type].Execute(cardActivity));
    }

    public string DescriptionByCardActivity(CardActivitySetting cardActivity)
    {
        if (!ActivityTypeToAction.ContainsKey(cardActivity.type))
        {
            Debug.LogError("No description exists for Activity of type" + cardActivity.type.ToString());
            return "_NULL";
        }
        return ActivityTypeToAction[cardActivity.type].GetDescription(cardActivity);
    }


    public string ToolTipByCardActivity(CardActivitySetting cardActivity)
    {
        if (!ActivityTypeToAction.ContainsKey(cardActivity.type))
        {
            Debug.LogError("No description exists for Activity of type" + cardActivity.type.ToString());
            return "_NULL";
        }
        return ActivityTypeToAction[cardActivity.type].GetToolTip(cardActivity);
    }

}
