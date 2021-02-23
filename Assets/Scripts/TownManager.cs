using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TownManager : Manager, ISaveable
{
    public List<TownInteractable> townEncounters;
    public List<BuildingType> unlockedBuildings = new List<BuildingType>();
    public List<BuildingType> startingBuildings = new List<BuildingType>();
    public BuildingTownHall buildingTownHall;
    public BuildingBarracks buildingBarracks;
    public Canvas townMapCanvas;
    protected override void Start()
    {
        base.Start();
        townMapCanvas.gameObject.SetActive(true);
    }

    public void EnterTown()
    {
        townMapCanvas.gameObject.SetActive(true);
        world.worldStateManager.AddState(WorldState.Town, true);
    }

    public void ExitTown()
    {
        townMapCanvas.gameObject.SetActive(false);
        // world.encounterManager.GenerateMap();
        // world.characterManager.characterVariablesUI.UpdateUI();
        // world.encounterManager.currentEncounter = world.encounterManager.overworldEncounters[0];
        // world.worldStateManager.AddState(WorldState.Overworld, true);
    }

    public bool UnlockBuilding(BuildingType building)
    {
        if (!unlockedBuildings.Contains(building))
        {
            unlockedBuildings.Add(building);
            return true;
        }
        else
        {
            Debug.Log("Already own this building!");
            return false;
        }
    }
    public void UpdateTown()
    {
        List<BuildingType> allBuildings = unlockedBuildings.Union(startingBuildings).ToList();

        foreach (TownInteractable townInt in townEncounters)
        {
            if (allBuildings.Contains(townInt.buildingType))
            {
                ActivateBuilding(townInt);
            }
            else
            {
                DeactivateBuilding(townInt);
            }
        }
    }

    public void ActivateBuilding(TownInteractable building)
    {
        building.gameObject.SetActive(true);
    }

    public void DeactivateBuilding(TownInteractable building)
    {
        building.gameObject.SetActive(false);
    }

    public void PopulateSaveData(SaveData a_SaveData)
    {
        a_SaveData.unlockedBuildings = unlockedBuildings;
        
    }


    public void LoadFromSaveData(SaveData a_SaveData)
    {
        unlockedBuildings = a_SaveData.unlockedBuildings;

    }
}
