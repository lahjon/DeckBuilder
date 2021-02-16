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
    public Canvas townMapCanvas;
    void Start()
    {
        townMapCanvas.gameObject.SetActive(true);
    }

    public void EnterTown()
    {
        townMapCanvas.gameObject.SetActive(true);
        WorldSystem.instance.worldStateManager.AddState(WorldState.Town, true);
    }

    public void ExitTown()
    {
        townMapCanvas.gameObject.SetActive(false);
        // WorldSystem.instance.encounterManager.GenerateMap();
        // WorldSystem.instance.characterManager.characterVariablesUI.UpdateUI();
        // WorldSystem.instance.encounterManager.currentEncounter = WorldSystem.instance.encounterManager.overworldEncounters[0];
        // WorldSystem.instance.worldStateManager.AddState(WorldState.Overworld, true);
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
        allBuildings.ForEach(x => Debug.Log(x));

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
        Debug.Log("Activated building!");
    }

    public void DeactivateBuilding(TownInteractable building)
    {
        building.gameObject.SetActive(false);
        Debug.Log("Deactivated building!");
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
