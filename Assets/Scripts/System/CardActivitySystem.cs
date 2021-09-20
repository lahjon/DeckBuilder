using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class CardActivitySystem : MonoBehaviour
{
    public static CardActivitySystem instance = null;
    public Dictionary<CardActivityType, CardActivity> ActivityTypeToAction = new Dictionary<CardActivityType, CardActivity>();


    private void Awake()
    {
        if (instance != null) Destroy(gameObject);
        else instance = this;
        CardActivity.ActivitySystem = this;
        ActivityTypeToAction[CardActivityType.DrawCard] = new CardActivityDrawCard();
        ActivityTypeToAction[CardActivityType.AddCardToCombat] = new CardActivityAddCardToCombat();
        ActivityTypeToAction[CardActivityType.Splice] = new CardActivitySplice();
        ActivityTypeToAction[CardActivityType.ExhaustDiscard] = new CardActivityExhaustDiscard();
        ActivityTypeToAction[CardActivityType.CombatCostChange] = new CardActivityCombatCostChange();
        ActivityTypeToAction[CardActivityType.SetRandomBroken] = new CardActivitySetRandomBroken();
        ActivityTypeToAction[CardActivityType.Heal] = new CardActivityHeal();
        ActivityTypeToAction[CardActivityType.ModifyEnergy] = new CardActivityModifyEnergy();
        ActivityTypeToAction[CardActivityType.DrawDiscard] = new CardActivityDrawDiscard();
        ActivityTypeToAction[CardActivityType.DualWield] = new CardActivityDualWield();
    }

    public void Start()
    {
    }

    public IEnumerator StartByCardActivity(CardActivitySetting cardActivity)
    {
        //Debug.Log("Starting card Activity:" + cardActivity.type);
        if (!ActivityTypeToAction.ContainsKey(cardActivity.type))
        {
            Debug.LogError("No function exists for Activity of type" + cardActivity.type.ToString());
        }
        else
            yield return StartCoroutine(ActivityTypeToAction[cardActivity.type].Execute(cardActivity.parameter));
    }

    public string DescriptionByCardActivity(CardActivitySetting cardActivity)
    {
        if (!ActivityTypeToAction.ContainsKey(cardActivity.type))
        {
            Debug.LogError("No description exists for Activity of type" + cardActivity.type.ToString());
            return "_NULL";
        }
        return ActivityTypeToAction[cardActivity.type].GetDescription(cardActivity.parameter);
    }


    public string ToolTipByCardActivity(CardActivitySetting cardActivity)
    {
        if (!ActivityTypeToAction.ContainsKey(cardActivity.type))
        {
            Debug.LogError("No description exists for Activity of type" + cardActivity.type.ToString());
            return "_NULL";
        }
        return ActivityTypeToAction[cardActivity.type].GetToolTip(cardActivity.parameter);
    }

}
