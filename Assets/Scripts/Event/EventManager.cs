using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventManager : Manager
{
    public delegate void EnterBuildingEvent(BuildingType building);
    public static event EnterBuildingEvent OnEnterBuildingEvent;

    public delegate void CompleteTileEvent(HexTile tile);
    public static event CompleteTileEvent OnCompleteTileEvent;

    public delegate void CompleteStorySegmentEvent(ScenarioSegment segment);
    public static event CompleteStorySegmentEvent OnCompleteStorySegmentEvent;

    public delegate void EncounterCompletedEvent(Encounter enc);
    public static event EncounterCompletedEvent OnEncounterCompletedEvent;

    public delegate void EncounterDataCompletedEvent(EncounterData data);
    public static event EncounterDataCompletedEvent OnEncounterDataCompletedEvent;

    public delegate void EnemyKilledEvent(CombatActorEnemy enemy);
    public static event EnemyKilledEvent OnEnemyKilledEvent;

    public delegate void StatModifiedEvent(StatType statType);
    public static event StatModifiedEvent OnStatModifiedEvent;

    public delegate void EnemyKilledNoArgEvent();
    public static event EnemyKilledNoArgEvent OnEnemyKilledNoArgEvent;

    public delegate void StatsTrackerUpdatedEvent();
    public static event StatsTrackerUpdatedEvent OnStatsTrackerUpdatedEvent;

    public delegate void HealthChangedEvent(int amount);
    public static event HealthChangedEvent OnHealthChangedEvent;
    public delegate void ExperienceChangedEvent(int amount);
    public static event ExperienceChangedEvent OnExperienceChangedEvent;

    public delegate void ActorLostLifeEvent(CombatActor actor, int amount);
    public static event ActorLostLifeEvent OnActorLostLifeEvent;

    public delegate void HealthChangedEventNoArg();
    public static event HealthChangedEventNoArg OnHealthChangedEventnoArg;

    public delegate void CompleteSpecialEventEvent(int eventId);
    public static event CompleteSpecialEventEvent OnCompleteSpecialEventEvent;

    public delegate void LevelUpEvent(int level);
    public static event LevelUpEvent OnLevelUpEvent;

    public delegate void CurrencyChangedEvent(CurrencyType classType, int currentAmount);
    public static event CurrencyChangedEvent OnCurrencyChanged;

    public delegate void NewWorldStateEvent(WorldState worldState);
    public static event NewWorldStateEvent OnNewWorldStateEvent;
    public delegate void NewOverlayStateEvent(OverlayState overlayState);
    public static event NewOverlayStateEvent OnNewOverlayStateEvent;
    public delegate void CombatWonEvent();
    public static event CombatWonEvent OnCombatWonEvent;
    public delegate void CompleteWorldEncounterEvent();
    public static event CompleteWorldEncounterEvent OnCompleteWorldEncounterEvent;

    public delegate void CardPlayNoArgEvent();
    public static event CardPlayNoArgEvent OnCardPlayNoArgEvent;

    public delegate void CardPlayEvent(Card card);
    public static event CardPlayEvent OnCardPlayEvent;

    public delegate void CardPlayTypeEvent(CardType type);
    public static event CardPlayTypeEvent OnCardPlayTypeEvent;

    public delegate void EnergyChangedEvent();
    public static event EnergyChangedEvent OnEnergyChangedEvent;

    public delegate void EnergyInfoChangedEvent(Dictionary<EnergyType, int> changes);
    public static event EnergyInfoChangedEvent OnEnergyInfoChangedEvent;

    public delegate void DeckCountChangeEvent();
    public static event DeckCountChangeEvent OnDeckCountChangeEvent;

    public delegate void DiscardCountChangeEvent();
    public static event DiscardCountChangeEvent OnDiscardCountChangeEvent;

    public delegate void HandCountChangeEvent();
    public static event HandCountChangeEvent OnHandCountChangeEvent;

    public delegate void WorldEncounterSegmentProgressEvent(string id);
    public static event WorldEncounterSegmentProgressEvent OnWorldEncounterSegmentProgressEvent;

    public delegate void TurnEndEvent();
    public static event TurnEndEvent OnTurnEndEvent;

    public static void EnterBuilding(BuildingType building)
    {
        if(OnEnterBuildingEvent != null)
            EventManager.OnEnterBuildingEvent(building);
    }

    public static void CurrencyChanged(CurrencyType currencyType, int currentAmount)
    {
        if(OnCurrencyChanged != null)
            EventManager.OnCurrencyChanged(currencyType, currentAmount);
    }

    public static void EnemyKilled(CombatActorEnemy enemy)
    {
        OnEnemyKilledEvent?.Invoke(enemy);
        OnEnemyKilledNoArgEvent?.Invoke();
    }

    public static void NewWorldState(WorldState state)
    {
        if(OnNewWorldStateEvent != null)
            EventManager.OnNewWorldStateEvent(state);
    }
    public static void StatModified(StatType statType)
    {
        OnStatModifiedEvent?.Invoke(statType);
    }

        public static void ExperiencedChanged(int amount)
    {
        OnExperienceChangedEvent?.Invoke(amount);
    }

    public static void NewOverlayState(OverlayState state)
    {
        if(OnNewOverlayStateEvent != null)
            EventManager.OnNewOverlayStateEvent(state);
    }

    public static void StatsTrackerUpdated()
    {
        if (OnStatsTrackerUpdatedEvent != null)
            EventManager.OnStatsTrackerUpdatedEvent();
    }
    public static void CompleteWorldScenario()
    {
        OnCompleteWorldEncounterEvent?.Invoke();
    
    }

    public static void CompleteSpecialEvent(int eventId)
    {
        if(OnCompleteSpecialEventEvent != null)
            EventManager.OnCompleteSpecialEventEvent(eventId);
    }
    public static void LevelUp(int level)
    {
        if(OnLevelUpEvent != null)
            EventManager.OnLevelUpEvent(level);
    }
    public static void CombatWon()
    {
        Debug.Log("Won Combat!");
        if(OnCombatWonEvent != null)
            EventManager.OnCombatWonEvent();
    }

    public static void ActorLostLife(CombatActor actor, int amount) => OnActorLostLifeEvent?.Invoke(actor, amount);
    
    public static void TileCompleted(HexTile tile)
    {
        OnCompleteTileEvent?.Invoke(tile);
    }

    public static void HealthChanged(int amount)
    {
        Debug.Log("Event HealthChanged");
        OnHealthChangedEvent?.Invoke(amount);
        OnHealthChangedEventnoArg?.Invoke();
    }

    public static void CardFinished(Card card)
    {
        OnCardPlayEvent?.Invoke(card);
        OnCardPlayTypeEvent?.Invoke(card.cardType);
        OnCardPlayNoArgEvent?.Invoke();
    }

    public static void EnergyChanged()
    {
        OnEnergyChangedEvent?.Invoke();
    }

    public static void EnergyInfoChanged(Dictionary<EnergyType, int> changes)
    {
        OnEnergyInfoChangedEvent?.Invoke(changes);
    }

    public static void DeckCountChanged()
    {
        OnDeckCountChangeEvent?.Invoke();
    }

    public static void DiscardCountChanged()
    {
        OnDiscardCountChangeEvent?.Invoke();
    }

    public static void HandCountChanged()
    {
        OnHandCountChangeEvent?.Invoke();
    }

    public static void WorldEncounterSegmentProgressed(string id)
    {
        OnWorldEncounterSegmentProgressEvent?.Invoke(id);
    }

    public static void EncounterCompleted(Encounter enc)
    {
        OnEncounterCompletedEvent?.Invoke(enc);
        if (enc.encData != null)
            OnEncounterDataCompletedEvent?.Invoke(enc.encData);
    }

    public static void StorySegmentCompleted(ScenarioSegment segment)
    {
        OnCompleteStorySegmentEvent?.Invoke(segment);
    }

    public static void TurnEnded()
    {
        OnTurnEndEvent?.Invoke();
    }

}


public interface IEventSubscriber
{
    void Subscribe();
    void Unsubscribe();
}