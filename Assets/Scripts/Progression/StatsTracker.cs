using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StatsTracker : MonoBehaviour, IEvents, ISaveable
{
    public static Dictionary<string, int> enemyTracker = new Dictionary<string, int>();
    public static Dictionary<BuildingType, int> buildingTracker = new Dictionary<BuildingType, int>();

    void Start()
    {
        Subscribe();
    }

    public void LoadFromSaveData(SaveData a_SaveData)
    {
        buildingTracker.SetDictionaryFromLists(a_SaveData.buildingTrackerKey, a_SaveData.buildingTrackerValues);
    }


    public void PopulateSaveData(SaveData a_SaveData)
    {
        buildingTracker.SetListsFromDictionary(ref a_SaveData.buildingTrackerKey, ref a_SaveData.buildingTrackerValues);
    }

    public void Subscribe()
    {
        EventManager.OnEnemyKilledEvent += EnemyKilled;
        EventManager.OnEnterBuildingEvent += EnterBuilding;
    }
    public void Unsubscribe()
    {
        EventManager.OnEnemyKilledEvent -= EnemyKilled;
        EventManager.OnEnterBuildingEvent -= EnterBuilding;
    }
    void EnemyKilled(EnemyData enemyData)
    {
        if (enemyTracker.ContainsKey(enemyData.enemyId))
        {
            enemyTracker[enemyData.enemyId]++;
        }
        else
        {
            enemyTracker.Add(enemyData.enemyId, 1);
        }
        EventManager.StatsTrackerUpdated();
    }
    void EnterBuilding(BuildingType buildingType)
    {
        if (buildingTracker.ContainsKey(buildingType))
        {
            buildingTracker[buildingType]++;
        }
        else
        {
            buildingTracker.Add(buildingType, 1);
        }
        EventManager.StatsTrackerUpdated();

        foreach (var item in buildingTracker)
        {
            Debug.Log(item.Value);
            Debug.Log(item.Key);
        }
    }
}
