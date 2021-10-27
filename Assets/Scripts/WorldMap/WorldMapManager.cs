using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WorldMapManager : Manager, ISaveableWorld
{
    public Canvas canvas;
    public int actIndex;
    public HashSet<string> availableWorldEncounters = new HashSet<string>();
    public HashSet<string> completedWorldEncounters = new HashSet<string>();
    public List<string> allWorldEncounters = new List<string>();
    public List<ScenarioData> startingEncounters = new List<ScenarioData>();
    public GameObject worldEncounterPrefab;
    public Transform encounterParent;
    public List<Scenario> worldEncounters;
    public WorldEncounterTooltip worldEncounterTooltip;
    public WorldMapConfirmWindow worldMapConfirmWindow;
    public Scenario currentWorldEncounter;
    protected override void Awake()
    {
        base.Awake();
        world.worldMapManager = this;
        for (int i = 0; i < encounterParent.childCount; i++)
        {
            Scenario enc = encounterParent.GetChild(i).GetComponent<Scenario>();
            enc.gameObject.SetActive(false);
            enc.worldEncounterData = DatabaseSystem.instance.scenarios.FirstOrDefault(x => x.id.ToString() == enc.name);
            worldEncounters.Add(enc);
            allWorldEncounters.Add(enc.worldEncounterData.ScenarioName);
        }
    }

    protected override void Start()
    {
        base.Start();
        //AddDefaultEncounters();
    }

    public void OpenMap()
    {
        UpdateMap();
        canvas.gameObject.SetActive(true);
    }

    public void CloseMap()
    {
        canvas.gameObject.SetActive(false);
    }

    public void ButtonEnterTown()
    {
        WorldStateSystem.SetInTown(true);
    }

    public void UpdateMap()
    {
        foreach (Scenario enc in worldEncounters)
        {
            if (enc.worldEncounterData != null && availableWorldEncounters.Contains(enc.worldEncounterData.ScenarioName))
                enc.BindData();
        }
    }

    void AddDefaultEncounters()
    {
        if (availableWorldEncounters == null || availableWorldEncounters.Count == 0) 
        {
            startingEncounters.ForEach(x => availableWorldEncounters.Add(x.ScenarioName));
        }
    }

    public void PopulateSaveDataWorld(SaveDataWorld a_SaveData)
    {
        a_SaveData.availableWorldEncounters = availableWorldEncounters.ToList();
    }

    public void LoadFromSaveDataWorld(SaveDataWorld a_SaveData)
    {
        a_SaveData.availableWorldEncounters?.ForEach(x => availableWorldEncounters.Add(x));
        AddDefaultEncounters();
    }
}
