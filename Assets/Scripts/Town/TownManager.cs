using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class TownManager : Manager, ISaveableWorld
{
    [HideInInspector] public List<TownInteractable> townInteractables;
    public List<BuildingStruct> buildings;
    public List<BuildingType> unlockedBuildings = new List<BuildingType>();
    public List<BuildingType> startingBuildings = new List<BuildingType>();
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
            townInteractables.Add(encounters.GetChild(i).GetComponent<TownInteractable>());
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

        foreach (TownInteractable townInt in townInteractables)
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

    public Building GetBuildingByType(BuildingType type)
    {
        if (buildings.FirstOrDefault(x => x.buildingType == type) is BuildingStruct building) return building.building;
        return null;
    }
    public TownInteractable GetTownInteractableByType(BuildingType type)
    {
        if (townInteractables.FirstOrDefault(x => x.buildingType == type) is TownInteractable townInteractable) return townInteractable;
        return null;
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

[System.Serializable] public struct BuildingStruct
{
    public BuildingType buildingType;
    public Building building;
}
