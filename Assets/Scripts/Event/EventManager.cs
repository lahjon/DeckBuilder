using UnityEngine;
using System.Collections;

public class EventManager : Manager 
{
    public delegate void EnterBuildingEvent(BuildingType building);
    public static event EnterBuildingEvent OnEnterBuildingEvent;

    public delegate void EnemyKilledEvent(EnemyData enemy);
    public static event EnemyKilledEvent OnEnemyKilledEvent;

    public delegate void StatsTrackerUpdatedEvent();
    public static event StatsTrackerUpdatedEvent OnStatsTrackerUpdatedEvent;

    public delegate void LevelUpEvent();
    public static event LevelUpEvent OnLevelUpEvent;

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

    public static void StatsTrackerUpdated()
    {
        if(OnStatsTrackerUpdatedEvent != null)
            EventManager.OnStatsTrackerUpdatedEvent();
    }
    public static void LevelUp()
    {
        if(OnLevelUpEvent != null)
            EventManager.OnLevelUpEvent();
    }
}


public interface IEvents
{
    void Subscribe();
    void Unsubscribe();
}