using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WorldMapManager : Manager, ISaveableWorld
{
    public Canvas canvas;
    public int actIndex;
    public HashSet<int> availableWorldEncounters = new HashSet<int>();
    public HashSet<int> completedWorldEncounters = new HashSet<int>();
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
            if (DatabaseSystem.instance.scenarios.FirstOrDefault(x => x.id.ToString() == enc.name) is ScenarioData scenarioData)
            {
                enc.worldEncounterData = scenarioData;
                worldEncounters.Add(enc);
            }
        }
    }

    protected override void Start()
    {
        base.Start();
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

    public void UnlockScenario(int id)
    {
        if (!completedWorldEncounters.Contains(id) && !availableWorldEncounters.Contains(id))
            availableWorldEncounters.Add(id);
    }

    public void UpdateMap()
    {
        foreach (Scenario enc in worldEncounters)
        {
            availableWorldEncounters.ToList().ForEach(x => Debug.Log(x)); 
            if (enc.worldEncounterData != null && availableWorldEncounters.Contains(enc.worldEncounterData.id))
                enc.BindData();
        }
    }

    public void PopulateSaveDataWorld(SaveDataWorld a_SaveData)
    {
        a_SaveData.availableWorldEncounters = availableWorldEncounters.ToList();
        a_SaveData.completedWorldEncounters = completedWorldEncounters.ToList();
    }

    public void LoadFromSaveDataWorld(SaveDataWorld a_SaveData)
    {
        a_SaveData.availableWorldEncounters?.ForEach(x => availableWorldEncounters.Add(x));
        a_SaveData.completedWorldEncounters?.ForEach(x => completedWorldEncounters.Add(x));
    }
}
