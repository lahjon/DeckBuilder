using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class CombatActivitySystem : MonoBehaviour
{
    public static CombatActivitySystem instance = null;
    public static Dictionary<CombatActivityType, CombatActivity> ActivityTypeToAction = new Dictionary<CombatActivityType, CombatActivity>
    {
        { CombatActivityType.DrawCard, new CombatActivityDrawCard()},
        { CombatActivityType.AddCardToCombat, new CombatActivityAddCardToCombat()},
        { CombatActivityType.Splice, new CombatActivitySplice()},
        { CombatActivityType.ExhaustDiscard, new CombatActivityExhaustDiscard()},
        { CombatActivityType.CombatCostChange, new CombatActivityCombatCostChange()},
        { CombatActivityType.SetRandomBroken, new CombatActivitySetRandomBroken()},
        { CombatActivityType.ModifyLife, new CombatActivityModifyLife()},
        { CombatActivityType.ModifyEnergy, new CombatActivityModifyEnergy()},
        { CombatActivityType.DrawDiscard, new CombatActivityDrawDiscard()},
        { CombatActivityType.DualWield, new CombatActivityDualWield()},
        { CombatActivityType.EmptySavedEnergy, new CombatActivityEmptySavedEnergy()},
        { CombatActivityType.EN_AddCardModifier, new CombatActivityEN_AddCardModifier()},
        { CombatActivityType.AddCardModifier, new CombatActivityAddCardModifier()},
    };


    private void Awake()
    {
        if (instance != null) Destroy(gameObject);
        else instance = this;
        CombatActivity.ActivitySystem = this;
    }

    public IEnumerator StartByCardActivity(CombatActivitySetting cardActivity)
    {
        //Debug.Log("Starting card Activity:" + cardActivity.type);
        if (!ActivityTypeToAction.ContainsKey(cardActivity.type))
        {
            Debug.LogError("No function exists for Activity of type " + cardActivity.type.ToString());
        }
        else
            yield return StartCoroutine(ActivityTypeToAction[cardActivity.type].Execute(cardActivity));
    }

    public string DescriptionByCardActivity(CombatActivitySetting cardActivity)
    {
        if (!ActivityTypeToAction.ContainsKey(cardActivity.type))
        {
            Debug.LogError("No description exists for Activity of type" + cardActivity.type.ToString());
            return "_NULL";
        }
        return ActivityTypeToAction[cardActivity.type].GetDescription(cardActivity);
    }


    public string ToolTipByCardActivity(CombatActivitySetting cardActivity)
    {
        if (!ActivityTypeToAction.ContainsKey(cardActivity.type))
        {
            Debug.LogError("No description exists for Activity of type" + cardActivity.type.ToString());
            return "_NULL";
        }
        return ActivityTypeToAction[cardActivity.type].GetToolTip(cardActivity);
    }

}
