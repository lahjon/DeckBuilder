using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class ObjectiveManager : Manager, ISaveableWorld
{
    public List<ObjectiveData> clearedObjectives = new List<ObjectiveData>();
    public List<ObjectiveData> currentObjectives = new List<ObjectiveData>();
    public List<ObjectiveData> startingObjectives = new List<ObjectiveData>();
    public List<ObjectiveData> allObjectiveDatas;
    public GameObject objectivePrefab;

    protected override void Awake()
    {
        base.Awake(); 
        world.objectiveManager = this;      
    }

    protected override void Start()
    {
        base.Start();
    }

    // public void StartObjective(string objectiveId)
    // {
    //     if(!string.IsNullOrEmpty(objectiveId) && objectiveDatas.FirstOrDefault(x => x.name == objectiveId) is ObjectiveData data)
    //     {
    //         Objective obj = Instantiate(objectivePrefab, transform).GetComponent<Objective>();
    //         obj.name = data.id;
    //         currentObjectives.Add(data.id);
    //         obj.StartObjetive(data);
    //     }
    // }

    public Objective StartObjective(ObjectiveData data, bool fromLoad = false)
    {
        if (data == null || clearedObjectives.Contains(data)) return null;
        if (!fromLoad && clearedObjectives.Contains(data)) return null;
        if (!fromLoad) currentObjectives.Add(data);

        Objective obj = Instantiate(objectivePrefab, transform).GetComponent<Objective>();
        obj.name = data.id;
        obj.StartObjetive(data);
        return obj;
    }
    void RegisterObjectives()
    {
        List<ObjectiveData> allObjs = startingObjectives.Except(clearedObjectives).Except(currentObjectives).ToList();
        if (allObjs != null && allObjs.Any())
            allObjs.ForEach(x => StartObjective(x));
    }

    public void AddCompleteObjective(Objective objective)
    {
        if (!clearedObjectives.Contains(objective.data)) clearedObjectives.Add(objective.data);
        if (currentObjectives.Contains(objective.data)) currentObjectives.Remove(objective.data);

        world.SaveProgression();
    }

    public void PopulateSaveDataWorld(SaveDataWorld a_SaveData)
    {
        a_SaveData.clearedObjectives = clearedObjectives.Select(x => x.id).ToList();
        a_SaveData.currentObjectives = currentObjectives.Select(x => x.id).ToList();

        List<IntListWrapper> allGoals = new List<IntListWrapper>();
        //for (int i = transform.childCount - 1; i > 0 ; i--)
        for (int i = 0; i < transform.childCount; i++)
        {
            Objective obj = transform.GetChild(i).GetComponent<Objective>();
            IntListWrapper innerList = new IntListWrapper();
            innerList.aList = obj.countingConditions.Select(x => x.currentAmount).ToList();
            allGoals.Add(innerList);
        }

        Debug.Log("goal: " + allGoals.Count);
        a_SaveData.currentObjectiveGoals = allGoals;
    }

    public void LoadFromSaveDataWorld(SaveDataWorld a_SaveData)
    {
        if(a_SaveData.clearedObjectives != null)
        {   
            clearedObjectives = allObjectiveDatas.Where(x => a_SaveData.clearedObjectives.Contains(x.id)).ToList();
        }
        if(a_SaveData.currentObjectives != null)
        {   
            currentObjectives = allObjectiveDatas.Where(x => a_SaveData.currentObjectives.Contains(x.id)).ToList();
            currentObjectives.Reverse();
            for (int i = 0; i < currentObjectives.Count; i++)
            {
                Objective obj = StartObjective(currentObjectives[i], true);
                for (int g = 0; g < obj.countingConditions.Count; g++)
                {
                    obj.countingConditions[g].currentAmount = a_SaveData.currentObjectiveGoals[i][g];
                }
            }
        }
        RegisterObjectives();
    }
}