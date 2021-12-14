using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WorldMapManager : Manager, ISaveableWorld
{
    public Canvas canvas;
    public int actIndex;
    public List<int> availableScenarios = new List<int>();
    public List<int> availableHiddenScenarios = new List<int>();
    public List<int> completedScenarios = new List<int>();
    public Transform scenarioParent;
    public List<ScenarioUI> worldScenarios;
    public WorldScenarioTooltip worldScenarioTooltip;
    public WorldMapConfirmWindow worldMapConfirmWindow;
    public ScenarioData currentWorldScenarioData;
    public ScenarioUI currentScenarioUI;
    protected override void Awake()
    {
        base.Awake();
        world.worldMapManager = this;
        for (int i = 0; i < scenarioParent.childCount; i++)
        {
            ScenarioUI enc = scenarioParent.GetChild(i).GetComponent<ScenarioUI>();
            enc.gameObject.SetActive(false);
            if (DatabaseSystem.instance.scenarios.FirstOrDefault(x => x.id.ToString() == enc.name) is ScenarioData scenarioData)
            {
                enc.data = scenarioData;
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
        if (!completedScenarios.Contains(id) && !availableScenarios.Contains(id))
            availableScenarios.Add(id);
    }

    public void CompleteScenario(bool toTown = true)
    {
        EventManager.CompleteWorldScenario();
        CollectReward();
        RemoveScenario();
        if(toTown) 
        {
            world.characterManager.ResetDeck();
            WorldStateSystem.SetInTownReward(true);
            WorldStateSystem.SetInTown(true);
            WorldSystem.instance.scenarioMapManager.DeleteMap();
        }
    }
    void CollectReward()
    {
        Debug.Log("Collect Reward");
        if (currentWorldScenarioData.rewardStruct.type != RewardNormalType.None)
            WorldSystem.instance.rewardManager.CreateRewardNormal(currentWorldScenarioData.rewardStruct.type, currentWorldScenarioData.rewardStruct.value);
    }
    void RemoveScenario()
    {
        availableScenarios.Remove(currentScenarioUI.data.id);
        completedScenarios.Add(currentScenarioUI.data.id);
        if (currentScenarioUI.data.unlocksScenarios?.Any() == true)
            foreach (int id in currentScenarioUI.data.unlocksScenarios)
                UnlockScenario(id);


        worldScenarios.Remove(currentScenarioUI);
        currentScenarioUI.gameObject.SetActive(false);
        currentScenarioUI = null;
        
        WorldSystem.instance.worldMapManager.currentWorldScenarioData = null;
    }
    public void UpdateMap()
    {
        foreach (ScenarioUI enc in worldScenarios)
            if (enc.data != null && availableScenarios.Contains(enc.data.id))
                enc.BindData();
    }

    public void PopulateSaveDataWorld(SaveDataWorld a_SaveData)
    {
        a_SaveData.availableScenarios = availableScenarios.ToList();
        a_SaveData.completedScenarios = completedScenarios.ToList();
    }

    public void LoadFromSaveDataWorld(SaveDataWorld a_SaveData)
    {
        a_SaveData.availableScenarios?.ForEach(x => availableScenarios.Add(x));
        a_SaveData.completedScenarios?.ForEach(x => completedScenarios.Add(x));
    }
}
