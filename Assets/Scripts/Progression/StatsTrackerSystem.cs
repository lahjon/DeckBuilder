using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StatsTrackerSystem : MonoBehaviour, IEvents, ISaveableWorld
{
    public static Dictionary<string, int> enemyTracker = new Dictionary<string, int>();
    public static Dictionary<BuildingType, int> buildingTracker = new Dictionary<BuildingType, int>();
    public static Dictionary<CharacterClassType, int> characterLevels = new Dictionary<CharacterClassType, int>();
    public static List<string> completedEvents = new List<string>();
    public static StatsTrackerSystem instance;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        Subscribe();
    }

    public void LoadFromSaveDataWorld(SaveDataWorld a_SaveData)
    {
        buildingTracker.Clear();
        buildingTracker.SetDictionaryFromLists(a_SaveData.buildingTrackerKey, a_SaveData.buildingTrackerValues);
        characterLevels.SetDictionaryFromLists(a_SaveData.classTypes, a_SaveData.level);
        enemyTracker.SetDictionaryFromLists(a_SaveData.enemyId, a_SaveData.enemyAmountKilled);
    }


    public void PopulateSaveDataWorld(SaveDataWorld a_SaveData)
    {
        buildingTracker.SetListsFromDictionary(ref a_SaveData.buildingTrackerKey, ref a_SaveData.buildingTrackerValues);
        characterLevels.SetListsFromDictionary(ref a_SaveData.classTypes, ref a_SaveData.level);
        enemyTracker.SetListsFromDictionary(ref a_SaveData.enemyId, ref a_SaveData.enemyAmountKilled);
    }

    public void Subscribe()
    {
        EventManager.OnEnemyKilledEvent += EnemyKilled;
        EventManager.OnEnterBuildingEvent += EnterBuilding;
        EventManager.OnLevelUpEvent += LevelUp;
        EventManager.OnCompleteSpecialEventEvent += CompleteSpecialEvent;
    }
    public void Unsubscribe()
    {
        EventManager.OnEnemyKilledEvent -= EnemyKilled;
        EventManager.OnEnterBuildingEvent -= EnterBuilding;
        EventManager.OnLevelUpEvent -= LevelUp;
        EventManager.OnCompleteSpecialEventEvent -= CompleteSpecialEvent;
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
    }

    void CompleteSpecialEvent(string eventName)
    {
        if (eventName != null && !completedEvents.Contains(eventName))
        {
            completedEvents.Add(eventName);
        }
        
        EventManager.StatsTrackerUpdated();
    }

    void LevelUp(CharacterClassType classType, int level)
    {
        if (characterLevels.ContainsKey(classType))
        {
            characterLevels[classType] = level;
        }
        else
        {
            characterLevels.Add(classType, level);
        }
        EventManager.StatsTrackerUpdated();
    }
}
