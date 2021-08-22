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
    public List<WorldEncounterData> startingEncounters = new List<WorldEncounterData>();
    public GameObject worldEncounterPrefab;
    public Transform encounterParent;
    public List<WorldEncounter> worldEncounters;
    public WorldEncounterTooltip worldEncounterTooltip;
    public WorldMapConfirmWindow worldMapConfirmWindow;
    public WorldEncounter currentWorldEncounter;
    protected override void Awake()
    {
        base.Awake();
        world.worldMapManager = this;
        for (int i = 0; i < encounterParent.childCount; i++)
        {
            WorldEncounter enc = encounterParent.GetChild(i).GetComponent<WorldEncounter>();
            enc.gameObject.SetActive(false);
            worldEncounters.Add(enc);
            allWorldEncounters.Add(enc.worldEncounterData.worldEncounterName);
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
        foreach (WorldEncounter enc in worldEncounters)
        {
            if (availableWorldEncounters.Contains(enc.worldEncounterData.worldEncounterName))
            {
                enc.BindData();
                enc.gameObject.SetActive(true);
            }
        }
    }

    void AddDefaultEncounters()
    {
        if (availableWorldEncounters == null || availableWorldEncounters.Count == 0) 
        {
            startingEncounters.ForEach(x => availableWorldEncounters.Add(x.worldEncounterName));
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
