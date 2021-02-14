using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownManager : Manager, ISaveable
{
    public List<Encounter> townEncounters;
    public List<EncounterType> unlockedBuildings = new List<EncounterType>();
    public TownHall townHall;
    public Canvas townMapCanvas;
    void Start()
    {
        townMapCanvas.gameObject.SetActive(true);
        townEncounters.ForEach(x => x.UpdateEncounter());
    }
    public void EnterTavern()
    {

    }

    public void EnterPray()
    {
        
    }

    public void EnterBarracks()
    {
        
    }

    public void EnterTownHall()
    {
        WorldSystem.instance.worldStateManager.AddState(WorldState.Overworld, true);
        townHall.gameObject.SetActive(true);
        townHall.EnterBuilding();
    }

    public void LeaveTown()
    {

        townMapCanvas.gameObject.SetActive(false);
        WorldSystem.instance.encounterManager.canvas.gameObject.SetActive(true);
        WorldSystem.instance.encounterManager.GenerateMap();
        WorldSystem.instance.encounterManager.canvas.gameObject.SetActive(false);
        WorldSystem.instance.characterManager.characterVariablesUI.UpdateUI();
        WorldSystem.instance.encounterManager.currentEncounter = WorldSystem.instance.encounterManager.overworldEncounters[0];
        WorldSystem.instance.worldStateManager.AddState(WorldState.Overworld, true);
    }

    public void EnterShop()
    {
        
    }

    public bool UnlockBuilding(EncounterType building)
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
        foreach (Encounter enc in townEncounters)
        {
            if (!enc.gameObject.activeSelf && unlockedBuildings.Contains(enc.encounterType))
            {
                ActivateBuilding(enc);
            }
        }
    }

    public void ActivateBuilding(Encounter building)
    {
        building.gameObject.SetActive(true);
        Debug.Log("Activated building!");
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
