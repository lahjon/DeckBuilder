using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class ProgressionManager : Manager, ISaveable
{
    public HashSet<Objective> allClearedProgression = new HashSet<Objective>();
    private HashSet<Objective> allAttachedProgression = new HashSet<Objective>();
    public GameObject objectives;
    public bool isLoaded = false;

    protected override void Start()
    {
        base.Start(); 
        world.progressionManager = this;      
    }
    void RegisterObjectives()
    {
        for (int i = 0; i < objectives.transform.childCount; i++)
        {
            allAttachedProgression.Add(objectives.transform.GetChild(i).GetComponent<Objective>());
        }
        allAttachedProgression.ExceptWith(allClearedProgression);
        
        foreach (Objective objective in allClearedProgression)
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
        allClearedProgression.Add(objective);
        Debug.Log("Save me now :" + allClearedProgression.Count);
        world.SaveProgression();
    }

    public void PopulateSaveData(SaveData a_SaveData)
    {
        a_SaveData.allClearedProgression = allClearedProgression.ToArray();
    }

    public void LoadFromSaveData(SaveData a_SaveData)
    {
        if(a_SaveData.allClearedProgression != null)
        {
            allClearedProgression = new HashSet<Objective>(a_SaveData.allClearedProgression);
        }
        if (isLoaded)
        {
            RegisterObjectives();
        }
    }
}