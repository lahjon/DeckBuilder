using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class TownManager : Manager, ISaveableWorld
{
    public List<TownInteractable> townEncounters;
    public List<BuildingType> unlockedBuildings = new List<BuildingType>();
    public List<BuildingType> startingBuildings = new List<BuildingType>();
    public BuildingTownHall buildingTownHall;
    public BuildingBarracks buildingBarracks;
    public Canvas townMapCanvas;
    public Transform encounters;
    public Button worldMapButton;
    protected override void Awake()
    {
        base.Awake();
        Init();
    }
    public void OpenWorldMap()
    {
        WorldStateSystem.SetInTown(false);
        WorldStateSystem.SetInWorldMap(true);
    }

    void Init()
    {
        world.townManager = this;
        townMapCanvas.gameObject.SetActive(true);
        for (int i = 0; i < encounters.childCount ; i++)
        {
            townEncounters.Add(encounters.GetChild(i).GetComponent<TownInteractable>());
        }
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
    public void EnterTown()
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

        townMapCanvas.gameObject.SetActive(true);
    }

    public void ExitTown()
    {
        townMapCanvas.gameObject.SetActive(false);
    }

    public void ActivateBuilding(TownInteractable building)
    {
        building.gameObject.SetActive(true);
    }

    public void DeactivateBuilding(TownInteractable building)
    {
        building.gameObject.SetActive(false);
    }

    public void PopulateSaveDataWorld(SaveDataWorld a_SaveData)
    {
        a_SaveData.unlockedBuildings = unlockedBuildings;
        
    }
    public void LoadFromSaveDataWorld(SaveDataWorld a_SaveData)
    {
        unlockedBuildings = a_SaveData.unlockedBuildings;

    }
}