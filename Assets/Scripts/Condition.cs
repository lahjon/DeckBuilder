    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Condition : IEventSubscriber
{
    public ConditionData conditionData;
    public ConditionTypeInfo info;
    public bool value;

    public Action OnPreConditionUpdate;
    public Action OnConditionFlipTrue;
    public Action OnConditionFlipFalse;
    public Action OnConditionFlip;

    public Func<ConditionData, bool> ConditionEvaluator;

    public static implicit operator bool(Condition c) => c.value;

    public Condition(ConditionData conditionData, Action OnPreConditionUpdate = null, Action OnConditionFlip = null, Action OnConditionFlipTrue = null, Action OnConditionFlipFalse = null)
    {
        this.conditionData = conditionData;
        if (conditionData == null || conditionData.type == ConditionType.None)
        {
            value = true;
            return;
        }

        info = ConditionTypeInfo.GetConditionInfo(conditionData.type);
        ConditionEvaluator = info.conditionChecker;
        this.OnPreConditionUpdate = OnPreConditionUpdate;
        this.OnConditionFlip = OnConditionFlip;
        this.OnConditionFlipTrue = OnConditionFlipTrue;
        this.OnConditionFlipFalse = OnConditionFlipFalse;
    }

    public Condition()
    {
        conditionData = new ConditionData() { type = ConditionType.None };
        value = true;
    }

    public string GetTextCard() => conditionData.type == ConditionType.None ? "" : (info.GetTextInfo(conditionData) + ":\n");

    public virtual void Subscribe()
    {
        switch (conditionData.type)
        {
            case ConditionType.None:
                return;
            case ConditionType.CardsPlayedAtLeast:
            case ConditionType.CardsPlayedAtMost:
            case ConditionType.LastCardPlayedTurnType:
                EventManager.OnCardPlayNoArgEvent       += OnEventNotification;
                break;
            case ConditionType.WinCombat:
                EventManager.OnCombatWonEvent           += OnEventNotification;
                break;
            case ConditionType.ClearTile:
            case ConditionType.StoryTileCompleted:
                EventManager.OnCompleteTileEvent        += OnEventNotification;
                break;
            case ConditionType.KillEnemy:
                EventManager.OnEnemyKilledEvent         += OnEventNotification;
                break;
            case ConditionType.EnterBuilding:
                EventManager.OnEnterBuildingEvent       += OnEventNotification;
                break;
            case ConditionType.EncounterDataCompleted:
                EventManager.OnEncounterDataCompletedEvent += OnEventNotification;
                break;
            case ConditionType.EncounterCompleted:
                EventManager.OnEncounterCompletedEvent += OnEventNotification;
                break;
            case ConditionType.StorySegmentCompleted:
                EventManager.OnCompleteStorySegmentEvent += OnEventNotification;
                break;
            case ConditionType.SpendEnergySpecific:
                EventManager.OnEnergyInfoChangedEvent += OnEventNotification;
                break;
            case ConditionType.HealthPercentLessThan:
                EventManager.OnHealthChangedEventnoArg += OnEventNotification;
                break;
            case ConditionType.CardPlayType:
                EventManager.OnCardPlayTypeEvent += OnEventNotification;
                break;
            default:
                break;
        }

        if (this is ConditionCounting) return;
        OnEventNotification();
    }

    public virtual void Unsubscribe()
    {
        switch (conditionData.type)
        {
            case ConditionType.None:
                return;
            case ConditionType.CardsPlayedAtLeast:
            case ConditionType.CardsPlayedAtMost:
            case ConditionType.LastCardPlayedTurnType:
                EventManager.OnCardPlayNoArgEvent       -= OnEventNotification;
                break;
            case ConditionType.WinCombat:
                EventManager.OnCombatWonEvent           -= OnEventNotification;
                break;
            case ConditionType.ClearTile:
            case ConditionType.StoryTileCompleted:
                EventManager.OnCompleteTileEvent        -= OnEventNotification;
                break;
            case ConditionType.KillEnemy:
                EventManager.OnEnemyKilledEvent         -= OnEventNotification;
                break;
            case ConditionType.EnterBuilding:
                EventManager.OnEnterBuildingEvent       -= OnEventNotification;
                break;
            case ConditionType.EncounterDataCompleted:
                EventManager.OnEncounterDataCompletedEvent -= OnEventNotification;
                break;
            case ConditionType.EncounterCompleted:
                EventManager.OnEncounterCompletedEvent -= OnEventNotification;
                break;
            case ConditionType.SpendEnergySpecific:
                EventManager.OnEnergyInfoChangedEvent -= OnEventNotification;
                break;
            case ConditionType.HealthPercentLessThan:
                EventManager.OnHealthChangedEventnoArg -= OnEventNotification;
                break;
            case ConditionType.CardPlayType:
                EventManager.OnCardPlayTypeEvent -= OnEventNotification;
                break;
            default:
                break;
        }
    }
    public virtual void OnEventNotification()
    {
        if (ConditionEvaluator == null) return;
        bool oldVal = value;
        value = ConditionEvaluator.Invoke(conditionData);

        OnPreConditionUpdate?.Invoke();

        if (oldVal != value)
        {
            OnConditionFlip?.Invoke();
            if (oldVal)
                OnConditionFlipFalse?.Invoke();
            else
                OnConditionFlipTrue?.Invoke();
        }
    }

    public void OnEventNotification(EnemyData enemy)
    {
        if (string.IsNullOrEmpty(conditionData.strParameter) || enemy.enemyId == Int32.Parse(conditionData.strParameter) || conditionData.strParameters.Contains(enemy.enemyId.ToString()))
            OnEventNotification();
    }

    public void OnEventNotification(Dictionary<EnergyType, int> changes)
    {
        if (string.IsNullOrEmpty(conditionData.strParameter))
            OnEventNotification();
        else
        {
            EnergyType type = conditionData.strParameter.ToEnum<EnergyType>();
            if (changes.ContainsKey(type) && changes[type] >= 0)
                OnEventNotification();
        }
    }

    public void OnEventNotification(BuildingType buildingType)
    {
        if (string.IsNullOrEmpty(conditionData.strParameter) || buildingType.ToString() == conditionData.strParameter)
            OnEventNotification();
    }

    public void OnEventNotification(CardType cardType)
    {
        if (conditionData.strParameter == cardType.ToString())
            OnEventNotification();
    }

    public void OnEventNotification(HexTile tile)
    {
        if (string.IsNullOrEmpty(conditionData.strParameter) || tile.storyId == conditionData.strParameter)
            OnEventNotification();
    }

    public void OnEventNotification(EncounterData data)
    {
        if (data.name == conditionData.strParameter || conditionData.strParameters.Contains(data.name))
            OnEventNotification();
    }

    public void OnEventNotification(Encounter enc)
    {
        ScenarioEncounterType type;
        Enum.TryParse(conditionData.strParameter, out type);
        if(type == enc.encounterType || enc.storyID == conditionData.strParameter)
            OnEventNotification();
    }

    public void OnEventNotification(ScenarioSegment segment)
    {
        if (segment.data.SegmentName == conditionData.strParameter || conditionData.strParameters.Contains(segment.data.SegmentName))
            OnEventNotification();
    }
}


