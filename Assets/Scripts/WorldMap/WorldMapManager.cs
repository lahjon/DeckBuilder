using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WorldMapManager : Manager, ISaveableWorld
{
    public Canvas canvas;
    public int actIndex;
    public HashSet<int> availableWorldScenarios = new HashSet<int>();
    public HashSet<int> completedWorldScenarios = new HashSet<int>();
    public Transform scenarioParent;
    public List<Scenario> worldScenarios;
    public WorldEncounterTooltip worldScenarioTooltip;
    public WorldMapConfirmWindow worldMapConfirmWindow;
    public Scenario currentWorldScenario;
    protected override void Awake()
    {
        base.Awake();
        world.worldMapManager = this;
        for (int i = 0; i < scenarioParent.childCount; i++)
        {
            Scenario enc = scenarioParent.GetChild(i).GetComponent<Scenario>();
            enc.gameObject.SetActive(false);
            if (DatabaseSystem.instance.scenarios.FirstOrDefault(x => x.id.ToString() == enc.name) is ScenarioData scenarioData)
            {
                enc.worldScenarioData = scenarioData;
                worldScenarios.Add(enc);
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
        if (!completedWorldScenarios.Contains(id) && !availableWorldScenarios.Contains(id))
            availableWorldScenarios.Add(id);
    }

    public void CompleteScenario(bool toTown = true)
    {
        //if(aScenario != currentWorldScenario) return;
        currentWorldScenario.completed = true;
        EventManager.CompleteWorldEncounter();
        currentWorldScenario?.CollectReward();
        if(toTown) WorldStateSystem.SetInTown(true);
    }

    public void UpdateMap()
    {
        foreach (Scenario enc in worldScenarios)
        {
            if (enc.worldScenarioData != null && availableWorldScenarios.Contains(enc.worldScenarioData.id))
                enc.BindData();
        }
    }

    public void PopulateSaveDataWorld(SaveDataWorld a_SaveData)
    {
        a_SaveData.availableWorldEncounters = availableWorldScenarios.ToList();
        a_SaveData.completedWorldEncounters = completedWorldScenarios.ToList();
    }

    public void LoadFromSaveDataWorld(SaveDataWorld a_SaveData)
    {
        a_SaveData.availableWorldEncounters?.ForEach(x => availableWorldScenarios.Add(x));
        a_SaveData.completedWorldEncounters?.ForEach(x => completedWorldScenarios.Add(x));
    }
}
