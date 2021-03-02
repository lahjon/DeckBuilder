using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class ProgressionManager : Manager, ISaveableWorld
{
    public HashSet<string> allClearedProgression = new HashSet<string>();
    private HashSet<Objective> allAttachedProgression = new HashSet<Objective>();
    private HashSet<string> allAttachedProgressionId = new HashSet<string>();
    public GameObject objectives;

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
    void RegisterObjectives()
    {
        for (int i = 0; i < objectives.transform.childCount; i++)
        {
            Objective obj = objectives.transform.GetChild(i).GetComponent<Objective>();
            allAttachedProgression.Add(obj);
            allAttachedProgressionId.Add(obj.name);
        }

        HashSet<Objective> allDisabledProgressions = new HashSet<Objective>();

        foreach (Objective item in allAttachedProgression)
        {
            if (allClearedProgression.Contains(item.name))
            {
                Debug.Log(item);
                allDisabledProgressions.Add(item);
            }
        }

        allAttachedProgression.Except(allDisabledProgressions);
        
        foreach (Objective objective in allDisabledProgressions)
        {
            objective.completed = true;
            objective.gameObject.SetActive(false);
        }

        foreach (Objective objective in allAttachedProgression)
        {
            objective.Init();
        }
    }

    public void AddCompleteObjective(Objective objective)
    {
        allClearedProgression.Add(objective.objectiveId);
        Debug.Log("Save me now :" + allClearedProgression.Count);
        world.SaveProgression();
    }

    public void PopulateSaveDataWorld(SaveDataWorld a_SaveData)
    {
        a_SaveData.allClearedProgression = allClearedProgression.ToArray();
    }

    public void LoadFromSaveDataWorld(SaveDataWorld a_SaveData)
    {
        if(a_SaveData.allClearedProgression != null)
        {
            allClearedProgression = new HashSet<string>(a_SaveData.allClearedProgression);
        }
    }
}