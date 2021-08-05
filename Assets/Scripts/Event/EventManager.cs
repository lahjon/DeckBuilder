using UnityEngine;
using System.Collections;

public class EventManager : Manager
{
    public delegate void EnterBuildingEvent(BuildingType building);
    public static event EnterBuildingEvent OnEnterBuildingEvent;

    public delegate void CompleteTileEvent();
    public static event CompleteTileEvent OnCompleteTileEvent;

    public delegate void EnemyKilledEvent(EnemyData enemy);
    public static event EnemyKilledEvent OnEnemyKilledEvent;

    public delegate void StatsTrackerUpdatedEvent();
    public static event StatsTrackerUpdatedEvent OnStatsTrackerUpdatedEvent;

    public delegate void CompleteSpecialEventEvent(string eventName);
    public static event CompleteSpecialEventEvent OnCompleteSpecialEventEvent;

    public delegate void LevelUpEvent(CharacterClassType classType, int level);
    public static event LevelUpEvent OnLevelUpEvent;

    public delegate void NewWorldStateEvent(WorldState worldState);
    public static event NewWorldStateEvent OnNewWorldStateEvent;
    public delegate void NewOverlayStateEvent(OverlayState overlayState);
    public static event NewOverlayStateEvent OnNewOverlayStateEvent;
    public delegate void WinCombatEvent();
    public static event WinCombatEvent OnWinCombatEvent;

    public delegate void CardPlayEvent();
    public static event CardPlayEvent OnCardPlayEvent;

    public delegate void CardPlaySignEvent(Card card);
    public static event CardPlaySignEvent OnCardPlaySignEvent;

    public static void EnterBuilding(BuildingType building)
    {
        if(OnEnterBuildingEvent != null)
            EventManager.OnEnterBuildingEvent(building);
    }

    public static void EnemyKilled(EnemyData enemy)
    {
        if(OnEnemyKilledEvent != null)
            EventManager.OnEnemyKilledEvent(enemy);
    }

    public static void NewWorldState(WorldState state)
    {
        if(OnNewWorldStateEvent != null)
            EventManager.OnNewWorldStateEvent(state);
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
    public static void CompleteSpecialEvent(string eventName)
    {
        if(OnCompleteSpecialEventEvent != null)
            EventManager.OnCompleteSpecialEventEvent(eventName);
    }
    public static void LevelUp(CharacterClassType classType, int level)
    {
        if(OnLevelUpEvent != null)
            EventManager.OnLevelUpEvent(classType, level);
    }
    public static void WinCombat()
    {
        Debug.Log("Winning Combat!");
        if(OnWinCombatEvent != null)
            EventManager.OnWinCombatEvent();
    }

    public static void CompleteTile()
    {
        if(OnCompleteTileEvent != null)
            EventManager.OnCompleteTileEvent();
    }

    public static void CardPlay(Card card)
    {
        if (OnCardPlayEvent != null)
            EventManager.OnCardPlayEvent();

        if (OnCardPlaySignEvent != null)
            EventManager.OnCardPlaySignEvent(card);

    }
}


public interface IEvents
{
    void Subscribe();
    void Unsubscribe();
}