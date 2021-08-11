using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class ObjectiveManager : Manager, ISaveableWorld
{
    public List<string> clearedObjectives = new List<string>();
    public List<string> currentObjectives = new List<string>();
    public List<string> startingObjectives = new List<string>();
    public List<ObjectiveData> objectiveDatas;
    public GameObject objectivePrefab;

    protected override void Awake()
    {
        base.Awake(); 
        world.progressionManager = this;      
    }

    protected override void Start()
    {
        base.Start();
        RegisterObjectives();
    }

    public void StartObjective(ObjectiveData data)
    {
        Objective obj = Instantiate(objectivePrefab, transform).GetComponent<Objective>();
        obj.name = data.id;
        currentObjectives.Add(data.id);
        obj.StartObjetive(data);
    }
    void RegisterObjectives()
    {
        List<ObjectiveData> allObjs = objectiveDatas.Where(x => startingObjectives.Union(currentObjectives).Except(clearedObjectives).Contains(x.id)).ToList();
        if (allObjs != null && allObjs.Any())
            allObjs.ForEach(x => StartObjective(x));
    }

    public void AddCompleteObjective(Objective objective)
    {
        if (!clearedObjectives.Contains(objective.id)) clearedObjectives.Add(objective.id);
        if (currentObjectives.Contains(objective.id)) currentObjectives.Remove(objective.id);

        world.SaveProgression();
    }

    public void PopulateSaveDataWorld(SaveDataWorld a_SaveData)
    {
        a_SaveData.allClearedProgression = clearedObjectives;
    }

    public void LoadFromSaveDataWorld(SaveDataWorld a_SaveData)
    {
        if(a_SaveData.allClearedProgression != null)
        {
            clearedObjectives = a_SaveData.allClearedProgression;
        }
    }
}