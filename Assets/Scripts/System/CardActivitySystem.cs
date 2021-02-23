using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class CardActivitySystem : MonoBehaviour
{
    public static CardActivitySystem instance = null;
    public CombatController combatController;
    public Dictionary<CardActivityType, CardActivity> ActivityTypeToAction = new Dictionary<CardActivityType, CardActivity>();


    private void Awake()
    {
        if (instance != null) Destroy(gameObject);
        else instance = this;
    }

    public void Start()
    {
        combatController = WorldSystem.instance.combatManager.combatController;
        CardActivity.ActivitySystem = this;
        CardActivity.combatController = combatController;
        ActivityTypeToAction[CardActivityType.DrawCard] = new CardActivityDrawCard();
        ActivityTypeToAction[CardActivityType.AddCardToDeck] = new CardActivityAddCardToDeck();
    }

    public IEnumerator StartByCardActivity(CardActivitySetting cardActivity)
    {
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
